using Abp.Application.Services;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.LichChamSoc.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LichChamSocAppService :
    ApplicationService,
    ILichChamSocAppService
{
    private readonly IRepository<LichChamSoc> _lichChamSocRepository;
    private readonly IRepository<KhachHang> _khachHangRepository;
    private readonly IRepository<ThuCung> _thuCungRepository;
    private readonly IRepository<DichVu> _dichVuRepository;
    private readonly IRepository<BangGia> _bangGiaRepository;
    private readonly IRepository<NhanVien> _nhanVienRepository;

    public LichChamSocAppService(
        IRepository<LichChamSoc> lichChamSocRepository,
        IRepository<KhachHang> khachHangRepository,
        IRepository<ThuCung> thuCungRepository,
        IRepository<DichVu> dichVuRepository,
        IRepository<BangGia> bangGiaRepository,
        IRepository<NhanVien> nhanVienRepository
       )
    {
        _lichChamSocRepository = lichChamSocRepository;
        _khachHangRepository = khachHangRepository;
        _thuCungRepository = thuCungRepository;
        _dichVuRepository = dichVuRepository;
        _bangGiaRepository = bangGiaRepository;
        _nhanVienRepository = nhanVienRepository;
    }

    public async Task<LichChamSocDto> GetLichChamSoc(int id)
    {
        var result = await _lichChamSocRepository.GetAll()
            .Where(x => x.Id == id)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                ThuCungId = x.ThuCungId,
                TenThuCung = x.ThuCung != null ? x.ThuCung.TenThuCung : string.Empty,
                DichVuId = x.DichVuId,
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : string.Empty,
                BangGiaId = x.BangGiaId,
                NhanVienId = x.NhanVienId,
                TenNhanVien = x.NhanVien != null ? x.NhanVien.Hoten : string.Empty,
                KhachHangId = x.KhachHangId,
                TenKhachHang = x.KhachHang != null ? x.KhachHang.Hoten : string.Empty,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .FirstOrDefaultAsync();

        if (result == null)
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");

        return result;
    }

    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task<List<LichChamSocDto>> GetAll()
    {
        return await _lichChamSocRepository.GetAll()
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                ThuCungId = x.ThuCungId,
                TenThuCung = x.ThuCung != null ? x.ThuCung.TenThuCung : string.Empty,
                DichVuId = x.DichVuId,
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : string.Empty,
                BangGiaId = x.BangGiaId,
                NhanVienId = x.NhanVienId,
                TenNhanVien = x.NhanVien != null ? x.NhanVien.Hoten : string.Empty,
                KhachHangId = x.KhachHangId,
                TenKhachHang = x.KhachHang != null ? x.KhachHang.Hoten : string.Empty,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .OrderBy(x => x.ThoiGian)
            .ToListAsync();
    }

    public async Task Update(SuaLichChamSocDto input)
    {
        var lichChamSoc = await _lichChamSocRepository.FirstOrDefaultAsync(input.Id);
        if (lichChamSoc == null)
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");

        if (input.DichVuId <= 0)
            throw new UserFriendlyException("Vui lòng chọn dịch vụ.");

        if (input.BangGiaId <= 0)
            throw new UserFriendlyException("Vui lòng chọn bảng giá.");

        var bangGia = await _bangGiaRepository.FirstOrDefaultAsync(x =>
            x.Id == input.BangGiaId && x.DichVuId == input.DichVuId);

        if (bangGia == null)
            throw new UserFriendlyException("Bảng giá không thuộc dịch vụ đã chọn.");

        if (input.ThoiGian == default)
            throw new UserFriendlyException("Vui lòng chọn thời gian.");

        if (input.ThoiGian <= DateTime.Now)
            throw new UserFriendlyException("Không thể chọn thời gian đã qua.");

        if (input.NhanVienId.HasValue)
        {
            var thoiGianKetThuc = input.ThoiGian.AddMinutes(bangGia.ThoiGianPhut);

            var lichTrung = await _lichChamSocRepository.GetAll()
                .Where(x =>
                    x.Id != input.Id &&
                    x.NhanVienId == input.NhanVienId &&
                    x.TrangThai != TrangThaiLichChamSoc.DaHuy &&
                    x.TrangThai != TrangThaiLichChamSoc.BiTuChoi)
                .Join(
                    _bangGiaRepository.GetAll(),
                    lichKhac => lichKhac.BangGiaId,
                    bg => bg.Id,
                    (lichKhac, bg) => new { Lich = lichKhac, BangGia = bg })
                .AnyAsync(x =>
                    input.ThoiGian < x.Lich.ThoiGian.AddMinutes(x.BangGia.ThoiGianPhut) &&
                    thoiGianKetThuc > x.Lich.ThoiGian);

            if (lichTrung)
                throw new UserFriendlyException("Nhân viên đã có lịch khác trong khung giờ này.");
        }

        lichChamSoc.DichVuId = input.DichVuId;
        lichChamSoc.BangGiaId = input.BangGiaId;
        lichChamSoc.ThoiGian = input.ThoiGian;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task ChangeStatus(SuaTrangThaiLichChamSocDto input)
    {
        var lichChamSoc = await _lichChamSocRepository.FirstOrDefaultAsync(input.Id);

        if (lichChamSoc == null)
        {
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");
        }

        var trangThaiCu = lichChamSoc.TrangThai;
        var trangThaiMoi = input.TrangThai;

        if (trangThaiCu == TrangThaiLichChamSoc.HoanThanh ||
            trangThaiCu == TrangThaiLichChamSoc.DaHuy ||
            trangThaiCu == TrangThaiLichChamSoc.BiTuChoi)
        {
            throw new UserFriendlyException("Không thể thay đổi trạng thái lịch này.");
        }

        lichChamSoc.TrangThai = trangThaiMoi;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
    [AbpAuthorize]
    public async Task<int> Create(ThemLichChamSocDto input)
    {
        Logger.Info("===== CREATE 1 =====");

        if (input == null)
        {
            Logger.Error("INPUT NULL");
            throw new UserFriendlyException("Dữ liệu đặt lịch không hợp lệ.");
        }

        Logger.Info($"INPUT: ThuCungId={input.ThuCungId}, DichVuId={input.DichVuId}, BangGiaId={input.BangGiaId}, ThoiGian={input.ThoiGian}");

        if (input.ThuCungId <= 0)
            throw new UserFriendlyException("Vui lòng chọn thú cưng.");

        if (input.DichVuId <= 0)
            throw new UserFriendlyException("Vui lòng chọn dịch vụ.");

        if (input.BangGiaId <= 0)
            throw new UserFriendlyException("Vui lòng chọn bảng giá.");

        if (input.ThoiGian == default(DateTime))
            throw new UserFriendlyException("Vui lòng chọn ngày và giờ.");

        if (input.ThoiGian <= DateTime.Now)
            throw new UserFriendlyException("Không thể đặt lịch ở thời gian đã qua.");

        Logger.Info("===== CREATE 2 - INPUT OK =====");

        var userId = AbpSession.UserId;
        Logger.Info($"UserId = {userId}");

        if (!userId.HasValue)
        {
            Logger.Error("ABP SESSION USER ID NULL");
            throw new UserFriendlyException("Vui lòng đăng nhập để đặt lịch.");
        }

        Logger.Info("===== CREATE 3 - USER OK =====");

        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(x => x.UserId == userId.Value);
        Logger.Info($"KhachHang = {(khachHang == null ? "NULL" : khachHang.Id.ToString())}");

        if (khachHang == null)
            throw new UserFriendlyException("Không tìm thấy thông tin khách hàng.");

        Logger.Info("===== CREATE 4 - KHACH HANG OK =====");

        var thuCung = await _thuCungRepository.FirstOrDefaultAsync(x =>
            x.Id == input.ThuCungId &&
            x.KhachHangId == khachHang.Id &&
            x.TrangThai);

        Logger.Info($"ThuCung = {(thuCung == null ? "NULL" : thuCung.Id.ToString())}");

        if (thuCung == null)
            throw new UserFriendlyException("Thú cưng không thuộc tài khoản của bạn hoặc đã ngừng hoạt động.");

        Logger.Info("===== CREATE 5 - THU CUNG OK =====");

        var dichVu = await _dichVuRepository.FirstOrDefaultAsync(x =>
            x.Id == input.DichVuId &&
            x.TrangThai);

        Logger.Info($"DichVu = {(dichVu == null ? "NULL" : dichVu.Id.ToString())}");

        if (dichVu == null)
            throw new UserFriendlyException("Dịch vụ không tồn tại hoặc hiện không hoạt động.");

        Logger.Info("===== CREATE 6 - DICH VU OK =====");

        var bangGia = await _bangGiaRepository.FirstOrDefaultAsync(x =>
            x.Id == input.BangGiaId &&
            x.DichVuId == input.DichVuId);

        Logger.Info($"BangGia = {(bangGia == null ? "NULL" : bangGia.Id.ToString())}");

        if (bangGia == null)
            throw new UserFriendlyException("Bảng giá không thuộc dịch vụ đã chọn.");

        Logger.Info($"===== CREATE 7 - BANG GIA OK - ID={bangGia.Id} =====");

        var lichTrung = await _lichChamSocRepository.GetAll().AnyAsync(x =>
            x.ThuCungId == input.ThuCungId &&
            x.ThoiGian == input.ThoiGian &&
            x.TrangThai != TrangThaiLichChamSoc.DaHuy);

        Logger.Info($"LichTrung = {lichTrung}");

        if (lichTrung)
            throw new UserFriendlyException("Thú cưng đã có lịch chăm sóc vào thời gian này.");

        Logger.Info("===== CREATE 8 - KHONG TRUNG LICH =====");

        var lichChamSoc = new LichChamSoc
        {
            ThuCungId = input.ThuCungId,
            DichVuId = input.DichVuId,
            BangGiaId = input.BangGiaId,
            KhachHangId = khachHang.Id,
            NhanVienId = null,
            ThoiGian = input.ThoiGian,
            TrangThai = TrangThaiLichChamSoc.ChoXacNhan
        };

        Logger.Info("===== CREATE 9 - TAO ENTITY =====");

        var id = await _lichChamSocRepository.InsertAndGetIdAsync(lichChamSoc);

        Logger.Info($"===== CREATE 10 - INSERT OK - ID={id} =====");

        await CurrentUnitOfWork.SaveChangesAsync();

        Logger.Info("===== CREATE 11 - SAVE OK =====");

        return id;
    }
    public async Task<List<LichChamSocDto>> GetLichChamSocCuaToi()
    {
        var userId = AbpSession.UserId;
        if (!userId.HasValue)
            throw new UserFriendlyException("Vui lòng đăng nhập.");

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.UserId == userId.Value);

        if (khachHang == null)
            throw new UserFriendlyException("Không tìm thấy khách hàng.");

        return await _lichChamSocRepository
            .GetAll()
            .Where(x => x.KhachHangId == khachHang.Id)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                ThuCungId = x.ThuCungId,
                DichVuId = x.DichVuId,
                BangGiaId = x.BangGiaId,
                NhanVienId = x.NhanVienId,
                KhachHangId = x.KhachHangId,
                ThoiGian = x.ThoiGian,
                TenThuCung = x.ThuCung != null ? x.ThuCung.TenThuCung : string.Empty,
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : string.Empty,
                TrangThai = x.TrangThai
            })
            .ToListAsync();
    }

    public async Task<List<LichChamSocDto>> GetLichSuLichChamSocCuaToi()
    {
        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.UserId == AbpSession.GetUserId());

        if (khachHang == null)
            return new List<LichChamSocDto>();

        return await _lichChamSocRepository
            .GetAllIncluding(x => x.ThuCung, x => x.DichVu)
            .Where(x =>
                x.KhachHangId == khachHang.Id &&
                (x.TrangThai == TrangThaiLichChamSoc.HoanThanh ||
                 x.TrangThai == TrangThaiLichChamSoc.BiTuChoi ||
                 x.TrangThai == TrangThaiLichChamSoc.DaHuy))
            .OrderByDescending(x => x.ThoiGian)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                KhachHangId = x.KhachHangId,
                NhanVienId = x.NhanVienId,
                DichVuId = x.DichVuId,
                BangGiaId = x.BangGiaId,
                ThuCungId = x.ThuCungId,
                TenThuCung = x.ThuCung != null ? x.ThuCung.TenThuCung : null,
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : "",
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .ToListAsync();
    }
    public async Task PhanCongNhanVien(int lichChamSocId, int nhanVienId)
    {
        var lich = await _lichChamSocRepository.FirstOrDefaultAsync(lichChamSocId);
        if (lich == null)
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");

        if (lich.TrangThai != TrangThaiLichChamSoc.ChoXacNhan)
            throw new UserFriendlyException("Không thể phân công cho lịch chăm sóc này.");

        var nhanVien = await _nhanVienRepository.FirstOrDefaultAsync(x => x.Id == nhanVienId && x.Trangthai);
        if (nhanVien == null)
            throw new UserFriendlyException("Nhân viên không tồn tại hoặc không còn làm việc.");

        var bangGia = await _bangGiaRepository.FirstOrDefaultAsync(x => x.Id == lich.BangGiaId);
        if (bangGia == null)
            throw new UserFriendlyException("Không tìm thấy bảng giá của lịch chăm sóc.");

        var thoiGianBatDau = lich.ThoiGian;
        var thoiGianKetThuc = thoiGianBatDau.AddMinutes(bangGia.ThoiGianPhut);

        var lichTrung = await _lichChamSocRepository.GetAll()
            .Where(x =>
                x.Id != lichChamSocId &&
                x.NhanVienId == nhanVienId &&
                x.TrangThai != TrangThaiLichChamSoc.DaHuy)
            .Join(
                _bangGiaRepository.GetAll(),
                lichKhac => lichKhac.BangGiaId,
                bg => bg.Id,
                (lichKhac, bg) => new
                {
                    Lich = lichKhac,
                    BangGia = bg
                })
            .AnyAsync(x =>
                thoiGianBatDau < x.Lich.ThoiGian.AddMinutes(x.BangGia.ThoiGianPhut) &&
                thoiGianKetThuc > x.Lich.ThoiGian);

        if (lichTrung)
            throw new UserFriendlyException("Nhân viên đã được phân công cho lịch khác trong khung giờ này.");

        lich.NhanVienId = nhanVienId;
        lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;

        await _lichChamSocRepository.UpdateAsync(lich);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
    public async Task HuyLichChamSoc(int id)
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
        {
            throw new UserFriendlyException("Vui lòng đăng nhập.");
        }

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.UserId == userId.Value);

        if (khachHang == null)
        {
            throw new UserFriendlyException("Không tìm thấy khách hàng.");
        }

        var lichChamSoc = await _lichChamSocRepository
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.KhachHangId == khachHang.Id);

        if (lichChamSoc == null)
        {
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");
        }

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.ChoXacNhan &&
            lichChamSoc.TrangThai != TrangThaiLichChamSoc.DaXacNhan)
        {
            throw new UserFriendlyException(
                "Chỉ được hủy lịch đang chờ xác nhận hoặc đã xác nhận.");
        }

        lichChamSoc.TrangThai = TrangThaiLichChamSoc.DaHuy;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task TuChoiLichChamSoc(int id)
    {
        var lichChamSoc = await _lichChamSocRepository.FirstOrDefaultAsync(id);

        if (lichChamSoc == null)
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.ChoXacNhan &&
            lichChamSoc.TrangThai != TrangThaiLichChamSoc.DaXacNhan)
        {
            throw new UserFriendlyException("Chỉ có thể từ chối lịch đang chờ xác nhận hoặc đã xác nhận.");
        }

        lichChamSoc.TrangThai = TrangThaiLichChamSoc.BiTuChoi;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
}