using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.HoaDon.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class HoaDonAppService : ApplicationService, IHoaDonAppService
{
    private readonly IRepository<HoaDon, int> _hoaDonRepository;
    private readonly IRepository<HoaDonChiTiet, int> _hoaDonChiTietRepository;
    private readonly IRepository<LichChamSoc, int> _lichChamSocRepository;
    private readonly IRepository<KhachHang, int> _khachHangRepository;
    private readonly IRepository<Vip, int> _vipRepository;
    private readonly IRepository<CauHinhVip, int> _cauHinhVipRepository;

    public HoaDonAppService(
        IRepository<HoaDon, int> hoaDonRepository,
        IRepository<HoaDonChiTiet, int> hoaDonChiTietRepository,
        IRepository<LichChamSoc, int> lichChamSocRepository,
        IRepository<KhachHang, int> khachHangRepository,
        IRepository<Vip, int> vipRepository,
        IRepository<CauHinhVip, int> cauHinhVipRepository)
    {
        _hoaDonRepository = hoaDonRepository;
        _hoaDonChiTietRepository = hoaDonChiTietRepository;
        _lichChamSocRepository = lichChamSocRepository;
        _khachHangRepository = khachHangRepository;
        _vipRepository = vipRepository;
        _cauHinhVipRepository = cauHinhVipRepository;
    }

    // =====================================================
    // TẠO HÓA ĐƠN TỪ LỊCH CHĂM SÓC
    // =====================================================
    public async Task<int> ThemHoaDon(ThemHoaDonDto input)
    {
        if (input == null || input.LichChamSocId <= 0)
            throw new UserFriendlyException("Thông tin tạo hóa đơn không hợp lệ.");

        var lichChamSoc = await _lichChamSocRepository
            .GetAllIncluding(x => x.KhachHang, x => x.NhanVien, x => x.DichVu, x => x.BangGia)
            .FirstOrDefaultAsync(x => x.Id == input.LichChamSocId);

        if (lichChamSoc == null)
            throw new UserFriendlyException("Không tìm thấy lịch chăm sóc.");

        if (lichChamSoc.TrangThai.ToString() != "DaXacNhan")
            throw new UserFriendlyException("Chỉ có lịch chăm sóc đã được xác nhận mới có thể tạo hóa đơn.");

        if (!lichChamSoc.NhanVienId.HasValue)
            throw new UserFriendlyException("Lịch chăm sóc chưa được phân công nhân viên.");

        var daCoHoaDon = await _hoaDonRepository
            .GetAll()
            .AnyAsync(x => x.LichChamSocId == input.LichChamSocId);

        if (daCoHoaDon)
            throw new UserFriendlyException("Lịch chăm sóc này đã được lập hóa đơn.");

        if (lichChamSoc.BangGia == null)
            throw new UserFriendlyException("Không tìm thấy bảng giá của lịch chăm sóc.");

        var donGia = lichChamSoc.BangGia.Giadv;

        if (donGia <= 0)
            throw new UserFriendlyException("Giá dịch vụ không hợp lệ.");

        // =====================================================
        // TÌM KHÁCH HÀNG
        // =====================================================
        var khachHang = await _khachHangRepository
            .GetAllIncluding(x => x.Vip)
            .FirstOrDefaultAsync(x => x.Id == lichChamSoc.KhachHangId);

        if (khachHang == null)
            throw new UserFriendlyException("Không tìm thấy thông tin khách hàng.");

        // =====================================================
        // LẤY CẤU HÌNH VIP ĐANG HIỆU LỰC
        // =====================================================
        var ngayHienTai = DateTime.Now;
        decimal phanTramGiam = 0;

        if (khachHang.VipId.HasValue && khachHang.Vip != null)
        {
            var cauHinhVip = await _cauHinhVipRepository
                .GetAll()
                .Where(x =>
                    x.VipId == khachHang.VipId.Value &&
                    x.TuNgay <= ngayHienTai &&
                    (x.DenNgay == null || x.DenNgay >= ngayHienTai))
                .OrderByDescending(x => x.TuNgay)
                .FirstOrDefaultAsync();

            if (cauHinhVip != null)
            {
                phanTramGiam = cauHinhVip.PhanTramGiam;
            }
        }

        // =====================================================
        // TÍNH TIỀN
        // =====================================================
        var tienGiam = donGia * phanTramGiam / 100;
        var tongTien = donGia - tienGiam;

        // =====================================================
        // TẠO HÓA ĐƠN
        // =====================================================
        var hoaDon = new HoaDon(
            lichChamSoc.Id,
            lichChamSoc.NhanVienId.Value,
            lichChamSoc.KhachHangId,
            ngayHienTai,
            donGia,
            phanTramGiam,
            tienGiam,
            tongTien,
            "Chưa thanh toán");

        var hoaDonId = await _hoaDonRepository.InsertAndGetIdAsync(hoaDon);

        var chiTiet = new HoaDonChiTiet(
            hoaDonId,
            lichChamSoc.DichVuId,
            donGia,
            tongTien);

        await _hoaDonChiTietRepository.InsertAsync(chiTiet);
        await CurrentUnitOfWork.SaveChangesAsync();

        return hoaDonId;
    }

    // =====================================================
    // DANH SÁCH HÓA ĐƠN
    // =====================================================
    public async Task<List<HoaDonDto>> LayDanhSachHoaDon()
    {
        var hoaDons = await _hoaDonRepository
            .GetAll()
            .OrderByDescending(x => x.NgayLap)
            .ToListAsync();

        return hoaDons.Select(h => new HoaDonDto
        {
            Id = h.Id,
            KhachHangId = h.KhachHangId,
            NhanVienId = h.NhanVienId,
            NgayLap = h.NgayLap,
            TongTien = h.TongTien,
            TrangThai = h.TrangThai
        }).ToList();
    }

    // =====================================================
    // XEM CHI TIẾT HÓA ĐƠN
    // =====================================================
    public async Task<XemChiTietHoaDonDto> GetChiTietAsync(int hoaDonId)
    {
        var hoaDon = await _hoaDonRepository
            .GetAllIncluding(x => x.ChiTietHoaDons)
            .FirstOrDefaultAsync(x => x.Id == hoaDonId);

        if (hoaDon == null)
            throw new UserFriendlyException("Hóa đơn không tồn tại.");

        var chiTietHoaDons = hoaDon.ChiTietHoaDons.Select(chiTiet => new HoaDonChiTietDto
        {
            Id = chiTiet.Id,
            DichVuId = chiTiet.DichVuId,
            DonGia = chiTiet.DonGia,
            ThanhTien = chiTiet.ThanhTien
        }).ToList();

        return new XemChiTietHoaDonDto
        {
            Id = hoaDon.Id,
            KhachHangId = hoaDon.KhachHangId,
            NhanVienId = hoaDon.NhanVienId,
            NgayLap = hoaDon.NgayLap,
            TongTien = hoaDon.TongTien,
            TrangThai = hoaDon.TrangThai,
            ChiTietHoaDons = chiTietHoaDons
        };
    }

    // =====================================================
    // ĐỔI TRẠNG THÁI HÓA ĐƠN
    // =====================================================
    public async Task DoiTrangThaiHoaDon(DoiTrangThaiHoaDonDto input)
    {
        if (input == null || input.Id <= 0)
            throw new UserFriendlyException("Thông tin hóa đơn không hợp lệ.");

        var hoaDon = await _hoaDonRepository.FirstOrDefaultAsync(input.Id);

        if (hoaDon == null)
            throw new UserFriendlyException("Hóa đơn không tồn tại.");

        if (string.IsNullOrWhiteSpace(input.TrangThai))
            throw new UserFriendlyException("Trạng thái hóa đơn không hợp lệ.");

        if (hoaDon.TrangThai == "Đã hủy")
            throw new UserFriendlyException("Không thể thay đổi trạng thái hóa đơn đã hủy.");

        hoaDon.TrangThai = input.TrangThai;
        await _hoaDonRepository.UpdateAsync(hoaDon);

        if (input.TrangThai == "Đã thanh toán")
        {
            var khachHang = await _khachHangRepository
                .FirstOrDefaultAsync(x => x.Id == hoaDon.KhachHangId);

            if (khachHang != null)
                await CapNhatVip(khachHang);
        }

        await CurrentUnitOfWork.SaveChangesAsync();
    }

    // =====================================================
    // CẬP NHẬT CẤP VIP
    // =====================================================
    private async Task CapNhatVip(KhachHang khachHang)
    {
        var tongDaThanhToan = await _hoaDonRepository
            .GetAll()
            .Where(x => x.KhachHangId == khachHang.Id && x.TrangThai == "Đã thanh toán")
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var vips = await _vipRepository
            .GetAll()
            .ToListAsync();

        Vip vip = null;

        if (tongDaThanhToan >= 20000000)
            vip = vips.FirstOrDefault(x => x.CapVip == 5);
        else if (tongDaThanhToan >= 10000000)
            vip = vips.FirstOrDefault(x => x.CapVip == 4);
        else if (tongDaThanhToan >= 5000000)
            vip = vips.FirstOrDefault(x => x.CapVip == 3);
        else if (tongDaThanhToan >= 3000000)
            vip = vips.FirstOrDefault(x => x.CapVip == 2);
        else if (tongDaThanhToan >= 1000000)
            vip = vips.FirstOrDefault(x => x.CapVip == 1);

        khachHang.VipId = vip?.Id;

        await _khachHangRepository.UpdateAsync(khachHang);
    }

    // =====================================================
    // SỬA HÓA ĐƠN
    // =====================================================
    public async Task SuaHoaDon(SuaHoaDonDto input)
    {
        if (input == null || input.Id <= 0)
            throw new UserFriendlyException("Thông tin hóa đơn không hợp lệ.");

        var hoaDon = await _hoaDonRepository.FirstOrDefaultAsync(input.Id);

        if (hoaDon == null)
            throw new UserFriendlyException("Hóa đơn không tồn tại.");

        hoaDon.TrangThai = input.TrangThai;
        hoaDon.TongTien = input.TongTien;

        await _hoaDonRepository.UpdateAsync(hoaDon);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
}