using Abp;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.UI;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.HoaDon.Dto;
using Cuahangchamsocthucung.LichChamSoc.Dto;
using Cuahangchamsocthucung.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
[AbpAuthorize(PermissionNames.Pages_HoaDon)]
public class HoaDonAppService : ApplicationService, IHoaDonAppService
{
    private readonly IRepository<HoaDon, int> _hoaDonRepository;
    private readonly IRepository<HoaDonChiTiet, int> _hoaDonChiTietRepository;
    private readonly IRepository<LichChamSoc, int> _lichChamSocRepository;
    private readonly IRepository<KhachHang, int> _khachHangRepository;
    private readonly IRepository<Vip, int> _vipRepository;
    private readonly IRepository<CauHinhVip, int> _cauHinhVipRepository;
    private readonly INotificationPublisher _notificationPublisher;

    public HoaDonAppService(
        IRepository<HoaDon, int> hoaDonRepository,
        IRepository<HoaDonChiTiet, int> hoaDonChiTietRepository,
        IRepository<LichChamSoc, int> lichChamSocRepository,
        IRepository<KhachHang, int> khachHangRepository,
        IRepository<Vip, int> vipRepository,
        IRepository<CauHinhVip, int> cauHinhVipRepository,
        INotificationPublisher notificationPublisher)
    {
        _hoaDonRepository = hoaDonRepository;
        _hoaDonChiTietRepository = hoaDonChiTietRepository;
        _lichChamSocRepository = lichChamSocRepository;
        _khachHangRepository = khachHangRepository;
        _vipRepository = vipRepository;
        _cauHinhVipRepository = cauHinhVipRepository;
        _notificationPublisher = notificationPublisher;
    }
    [AbpAuthorize(PermissionNames.Pages_HoaDon)]
    // TẠO HÓA ĐƠN TỪ LỊCH CHĂM SÓC
    public async Task<int> ThemHoaDon(ThemHoaDonDto input)
    {
        if (input == null || input.LichChamSocId <= 0)
            throw new UserFriendlyException("Thông tin tạo hóa đơn không hợp lệ.");

        var lichChamSoc = await _lichChamSocRepository
            .GetAllIncluding(x => x.KhachHang, x => x.NhanVien, x => x.DichVu, x => x.BangGia)
            .FirstOrDefaultAsync(x => x.Id == input.LichChamSocId);

        if (lichChamSoc == null)
            throw new UserFriendlyException("Không tìm thấy lịch chăm sóc.");

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.HoanThanh)
            throw new UserFriendlyException("Chỉ có lịch chăm sóc đã hoàn thành mới có thể tạo hóa đơn.");

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

        var khachHang = await _khachHangRepository
            .GetAllIncluding(x => x.Vip)
            .FirstOrDefaultAsync(x => x.Id == lichChamSoc.KhachHangId);

        if (khachHang == null)
            throw new UserFriendlyException("Không tìm thấy thông tin khách hàng.");

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
                phanTramGiam = cauHinhVip.PhanTramGiam;
        }

        if (phanTramGiam < 0)
            phanTramGiam = 0;

        if (phanTramGiam > 100)
            phanTramGiam = 100;

        var tienGiam = donGia * phanTramGiam / 100;
        var tongTien = donGia - tienGiam;

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
    [AbpAuthorize(PermissionNames.Pages_HoaDon)]

    // DANH SÁCH HÓA ĐƠN
    public async Task<List<HoaDonDto>> LayDanhSachHoaDon()
    {
        return await _hoaDonRepository
            .GetAll()
            .Include(x => x.KhachHang)
            .ThenInclude(x => x.Vip)
            .Include(x => x.NhanVien)
            .OrderByDescending(x => x.NgayLap)
            .Select(h => new HoaDonDto
            {
                Id = h.Id,
                KhachHangId = h.KhachHangId,
                TenKhachHang = h.KhachHang != null ? h.KhachHang.Hoten : string.Empty,
                NhanVienId = h.NhanVienId,
                TenNhanVien = h.NhanVien != null ? h.NhanVien.Hoten : string.Empty,
                NgayLap = h.NgayLap,
                TongTienTruocGiam = h.TongTienTruocGiam,
                PhanTramGiam = h.PhanTramGiam,
                TienGiam = h.TienGiam,
                TongTien = h.TongTien,
                TenVip = h.KhachHang != null && h.KhachHang.Vip != null ? h.KhachHang.Vip.TenVip : null,
                CapVip = h.KhachHang != null && h.KhachHang.Vip != null ? h.KhachHang.Vip.CapVip : null,
                TrangThai = h.TrangThai
            })
            .ToListAsync();
    }
    [AbpAuthorize(PermissionNames.Pages_HoaDon)]

    // XEM CHI TIẾT HÓA ĐƠN
    public async Task<XemChiTietHoaDonDto> GetChiTietAsync(int hoaDonId)
    {
        if (hoaDonId <= 0)
            throw new UserFriendlyException("Mã hóa đơn không hợp lệ.");

        var hoaDon = await _hoaDonRepository
            .GetAll()
            .Include(x => x.KhachHang)
                .ThenInclude(x => x.Vip)
            .Include(x => x.NhanVien)
            .Include(x => x.LichChamSoc)
                .ThenInclude(x => x.ThuCung)
            .Include(x => x.LichChamSoc)
                .ThenInclude(x => x.DichVu)
            .Include(x => x.LichChamSoc)
                .ThenInclude(x => x.BangGia)
                    .ThenInclude(x => x.DichVu)
            .Include(x => x.ChiTietHoaDons)
                .ThenInclude(x => x.DichVu)
            .FirstOrDefaultAsync(x => x.Id == hoaDonId);

        if (hoaDon == null)
            throw new UserFriendlyException("Hóa đơn không tồn tại.");

        if (hoaDon.LichChamSoc == null)
            throw new UserFriendlyException("Hóa đơn chưa có lịch chăm sóc.");

        var lich = hoaDon.LichChamSoc;
        var bangGia = lich.BangGia;

        if (bangGia == null)
            throw new UserFriendlyException("Không tìm thấy bảng giá được sử dụng.");

        var thoiGianTu = lich.ThoiGian;
        var thoiGianDen = thoiGianTu.AddMinutes(bangGia.ThoiGianPhut);

        var chiTietHoaDons = hoaDon.ChiTietHoaDons
            .Select(x => new HoaDonChiTietDto
            {
                Id = x.Id,
                HoaDonId = x.HoaDonId,
                DichVuId = x.DichVuId,
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : bangGia.DichVu?.TenDichVu,
                DonGia = x.DonGia,
                ThanhTien = x.ThanhTien
            })
            .ToList();

        return new XemChiTietHoaDonDto
        {
            Id = hoaDon.Id,

            KhachHangId = hoaDon.KhachHangId,
            TenKhachHang = hoaDon.KhachHang?.Hoten,
            SDTKhachHang = hoaDon.KhachHang?.SDT,

            NhanVienId = hoaDon.NhanVienId,
            TenNhanVien = hoaDon.NhanVien?.Hoten,

            ThuCungId = lich.ThuCungId,
            TenThuCung = lich.ThuCung?.TenThuCung,
            LoaiThuCung = lich.ThuCung?.LoaiThuCung,

            DichVuId = lich.DichVuId,
            TenDichVu = lich.DichVu?.TenDichVu ?? bangGia.DichVu?.TenDichVu,

            BangGiaId = bangGia.Id,
            TenBangGia = LayTenBangGia(bangGia),
            LoaiPhong = bangGia.LoaiPhong,
            LoaiLong = bangGia.Loailong ? "Lông dài" : "Lông ngắn",
            KhoangCanNang = $"{bangGia.Cannangtu}-{bangGia.Cannangden}kg",
            LoaiThuCungBangGia = bangGia.Loaithucung,
            DonGia = bangGia.Giadv,
            ThoiGianPhut = bangGia.ThoiGianPhut,

            NgayLap = hoaDon.NgayLap,
            ThoiGianTu = thoiGianTu,
            ThoiGianDen = thoiGianDen,

            TongTienTruocGiam = hoaDon.TongTienTruocGiam,
            PhanTramGiam = hoaDon.PhanTramGiam,
            TienGiam = hoaDon.TienGiam,
            TongTien = hoaDon.TongTien,

            TenVip = hoaDon.KhachHang?.Vip?.TenVip,
            CapVip = hoaDon.KhachHang?.Vip?.CapVip,

            TrangThai = hoaDon.TrangThai,

            ChiTietHoaDons = chiTietHoaDons
        };
    }
    [AbpAuthorize(PermissionNames.Pages_HoaDon)]

    // ĐỔI TRẠNG THÁI HÓA ĐƠN
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

        if (hoaDon.TrangThai == "Đã thanh toán" && input.TrangThai != "Đã thanh toán")
            throw new UserFriendlyException("Không thể thay đổi trạng thái của hóa đơn đã thanh toán.");

        var thanhToanMoi = hoaDon.TrangThai != "Đã thanh toán" && input.TrangThai == "Đã thanh toán";

        hoaDon.TrangThai = input.TrangThai;
        await _hoaDonRepository.UpdateAsync(hoaDon);

        if (thanhToanMoi)
        {
            var khachHang = await _khachHangRepository
                .FirstOrDefaultAsync(x => x.Id == hoaDon.KhachHangId);

            if (khachHang != null)
                await CapNhatVip(khachHang);
        }

        await CurrentUnitOfWork.SaveChangesAsync();
    }

    // CẬP NHẬT CẤP VIP
    private async Task CapNhatVip(KhachHang khachHang)
    {
        var tongDaThanhToan = await _hoaDonRepository.GetAll()
            .Where(x => x.KhachHangId == khachHang.Id &&
                        !x.IsDeleted &&
                        (x.TrangThai == "Đã thanh toán" || x.TrangThai == "DaThanhToan"))
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var ngayHienTai = DateTime.Now;

        var cauHinhVip = await _cauHinhVipRepository.GetAll()
            .Include(x => x.Vip)
            .Where(x => x.TenantId == khachHang.TenantId &&
                        x.TuNgay <= ngayHienTai &&
                        (x.DenNgay == null || x.DenNgay >= ngayHienTai) &&
                        x.MucChiTieu <= tongDaThanhToan)
            .OrderByDescending(x => x.Vip.CapVip)
            .ThenByDescending(x => x.MucChiTieu)
            .FirstOrDefaultAsync();

        if (cauHinhVip == null)
            return;

        var vipMoi = cauHinhVip.Vip;
        var capVipCu = 0;

        if (khachHang.VipId.HasValue)
        {
            capVipCu = await _vipRepository.GetAll()
                .Where(x => x.Id == khachHang.VipId.Value)
                .Select(x => x.CapVip)
                .FirstOrDefaultAsync();
        }

        if (vipMoi.CapVip > capVipCu)
        {
            khachHang.VipId = vipMoi.Id;
            await _khachHangRepository.UpdateAsync(khachHang);

            await _notificationPublisher.PublishAsync(
                AppNotificationNames.KhachHangDatVip,
                new MessageNotificationData(
                    $"Chúc mừng! Bạn đã được nâng lên {vipMoi.TenVip}. " +
                    $"Mức giảm giá hiện tại là {cauHinhVip.PhanTramGiam}%."
                ),
                userIds: new[]
                {
                new UserIdentifier(khachHang.TenantId, khachHang.UserId)
                });
        }
    }
    [AbpAuthorize(PermissionNames.Pages_HoaDon)]

    // SỬA HÓA ĐƠN
    public async Task SuaHoaDon(SuaHoaDonDto input)
    {
        if (input == null || input.Id <= 0)
            throw new UserFriendlyException("Thông tin hóa đơn không hợp lệ.");

        var hoaDon = await _hoaDonRepository.FirstOrDefaultAsync(input.Id);

        if (hoaDon == null)
            throw new UserFriendlyException("Hóa đơn không tồn tại.");

        if (hoaDon.TrangThai == "Đã thanh toán")
            throw new UserFriendlyException("Không thể sửa hóa đơn đã thanh toán.");

        if (hoaDon.TrangThai == "Đã hủy")
            throw new UserFriendlyException("Không thể sửa hóa đơn đã hủy.");

        if (string.IsNullOrWhiteSpace(input.TrangThai))
            throw new UserFriendlyException("Trạng thái hóa đơn không hợp lệ.");

        hoaDon.TrangThai = input.TrangThai;

        await _hoaDonRepository.UpdateAsync(hoaDon);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    // LẤY TÊN BẢNG GIÁ
    private string LayTenBangGia(BangGia bangGia)
    {
        if (bangGia == null)
            return "Không xác định";

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(bangGia.LoaiPhong))
            parts.Add(bangGia.LoaiPhong);

        if (!string.IsNullOrWhiteSpace(bangGia.Loaithucung))
            parts.Add(bangGia.Loaithucung);

        if (bangGia.Cannangtu >= 0 && bangGia.Cannangden > 0)
            parts.Add($"{bangGia.Cannangtu}-{bangGia.Cannangden}kg");

        parts.Add(bangGia.Loailong ? "Lông dài" : "Lông ngắn");

        return string.Join(" - ", parts);
    }
    [AbpAuthorize(PermissionNames.Pages_HoaDon)]

    // LẤY LỊCH ĐÃ HOÀN THÀNH CHƯA CÓ HÓA ĐƠN
    public async Task<List<LichChamSocChuaCoHoaDonDto>> LayLichChamSocChuaCoHoaDon()
    {
        var lichDaCoHoaDon = _hoaDonRepository.GetAll()
            .Where(x => !x.IsDeleted)
            .Select(x => x.LichChamSocId);

        return await _lichChamSocRepository
            .GetAll()
            .Include(x => x.KhachHang)
            .Include(x => x.NhanVien)
            .Include(x => x.ThuCung)
            .Include(x => x.DichVu)
            .Include(x => x.BangGia)
            .Where(x => x.TrangThai == TrangThaiLichChamSoc.HoanThanh && !lichDaCoHoaDon.Contains(x.Id))
            .OrderByDescending(x => x.ThoiGian)
            .Select(x => new LichChamSocChuaCoHoaDonDto
            {
                Id = x.Id,
                KhachHangId = x.KhachHangId,
                TenKhachHang = x.KhachHang != null ? x.KhachHang.Hoten : "",
                SDTKhachHang = x.KhachHang != null ? x.KhachHang.SDT : "",
                NhanVienId = x.NhanVienId,
                TenNhanVien = x.NhanVien != null ? x.NhanVien.Hoten : "",
                ThuCungId = x.ThuCungId,
                TenThuCung = x.ThuCung != null ? x.ThuCung.TenThuCung : "",
                DichVuId = x.DichVuId,
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : "",
                ThoiGian = x.ThoiGian,
                DonGia = x.BangGia != null ? x.BangGia.Giadv : 0,
                ThoiGianPhut = x.BangGia != null ? x.BangGia.ThoiGianPhut : 0
            })
            .ToListAsync();
    }
    [AbpAuthorize(PermissionNames.Pages_HoaDon)]
    public async Task XacNhanThanhToan(int id)
    {
        if (id <= 0)
            throw new UserFriendlyException("Mã hóa đơn không hợp lệ.");

        var hoaDon = await _hoaDonRepository.FirstOrDefaultAsync(id);

        if (hoaDon == null)
            throw new UserFriendlyException("Không tìm thấy hóa đơn.");

        if (hoaDon.TrangThai == "DaThanhToan")
            throw new UserFriendlyException("Hóa đơn đã được thanh toán.");

        if (hoaDon.TrangThai == "DaHuy")
            throw new UserFriendlyException("Không thể thanh toán hóa đơn đã hủy.");

        hoaDon.TrangThai = "DaThanhToan";

        await _hoaDonRepository.UpdateAsync(hoaDon);

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.Id == hoaDon.KhachHangId);

        if (khachHang != null)
            await CapNhatVip(khachHang);

        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [AbpAuthorize(PermissionNames.Pages_HoaDon)]
    public async Task HuyHoaDon(int id)
    {
        if (id <= 0)
            throw new UserFriendlyException("Mã hóa đơn không hợp lệ.");

        var hoaDon = await _hoaDonRepository.FirstOrDefaultAsync(id);

        if (hoaDon == null)
            throw new UserFriendlyException("Không tìm thấy hóa đơn.");

        if (hoaDon.TrangThai == "DaThanhToan")
            throw new UserFriendlyException("Không thể hủy hóa đơn đã thanh toán.");

        if (hoaDon.TrangThai == "DaHuy")
            throw new UserFriendlyException("Hóa đơn đã được hủy.");

        hoaDon.TrangThai = "DaHuy";

        await _hoaDonRepository.UpdateAsync(hoaDon);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
}