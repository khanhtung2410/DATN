using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
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

    public LichChamSocAppService(
        IRepository<LichChamSoc> lichChamSocRepository,
        IRepository<KhachHang> khachHangRepository,
        IRepository<ThuCung> thuCungRepository,
        IRepository<DichVu> dichVuRepository,
        IRepository<BangGia> bangGiaRepository
       )
    {
        _lichChamSocRepository = lichChamSocRepository;
        _khachHangRepository = khachHangRepository;
        _thuCungRepository = thuCungRepository;
        _dichVuRepository = dichVuRepository;
        _bangGiaRepository = bangGiaRepository;
    }

    public Task<LichChamSocDto> GetLichChamSoc(int id)
    {
        var query = _lichChamSocRepository.GetAll()
            .Where(x => x.Id == id)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                DichVuId = x.DichVuId,
                NhanVienId = x.NhanVienId,
                KhachHangId = x.KhachHangId,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            });
        return query.FirstOrDefaultAsync();
    }

    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public Task<List<LichChamSocDto>> GetAll()
    {
        var query = _lichChamSocRepository.GetAll()
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                DichVuId = x.DichVuId,
                NhanVienId = x.NhanVienId,
                KhachHangId = x.KhachHangId,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            });
        return query.ToListAsync();
    }

    public async Task Update(SuaLichChamSocDto input)
    {
        var lichChamSoc = await _lichChamSocRepository.FirstOrDefaultAsync(input.Id);
        if (lichChamSoc == null)
        {
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại");
        }
        if (input.NhanVienId.HasValue)
        {
            var lichChamSocCungNhanVien = _lichChamSocRepository.GetAll()
                .Where(x => x.NhanVienId == input.NhanVienId && x.Id != input.Id && x.ThoiGian == input.ThoiGian)
                .FirstOrDefault();
            if (lichChamSocCungNhanVien != null)
            {
                throw new UserFriendlyException("Nhân viên đã có lịch chăm sóc vào thời gian này");
            }
        }
        lichChamSoc.DichVuId = input.DichVuId;
        lichChamSoc.NhanVienId = input.NhanVienId;
        lichChamSoc.KhachHangId = input.KhachHangId;
        lichChamSoc.ThoiGian = input.ThoiGian;
        lichChamSoc.TrangThai = input.TrangThai;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public Task ChangeStatus(SuaTrangThaiLichChamSocDto input)
    {
        var lichChamSoc = _lichChamSocRepository.Get(input.Id);
        if (lichChamSoc == null)
        {
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại");
        }
        lichChamSoc.TrangThai = input.TrangThai;
        return _lichChamSocRepository.UpdateAsync(lichChamSoc);
    }

    public async Task<int> Create(ThemLichChamSocDto input)
    {
        try
        {
            Console.WriteLine("========== CREATE LICH CHAM SOC ==========");

            // 1. Kiểm tra input
            if (input == null)
            {
                throw new UserFriendlyException(
                    "Dữ liệu đặt lịch không hợp lệ.");
            }

            Console.WriteLine($"ThuCungId: {input.ThuCungId}");
            Console.WriteLine($"DichVuId: {input.DichVuId}");
            Console.WriteLine($"BangGiaId: {input.BangGiaId}");
            Console.WriteLine($"ThoiGian: {input.ThoiGian}");

            if (!input.ThuCungId.HasValue || input.ThuCungId.Value <= 0)
            {
                throw new UserFriendlyException(
                    "Vui lòng chọn thú cưng.");
            }

            if (!input.DichVuId.HasValue || input.DichVuId.Value <= 0)
            {
                throw new UserFriendlyException(
                    "Vui lòng chọn dịch vụ.");
            }

            if (!input.BangGiaId.HasValue || input.BangGiaId.Value <= 0)
            {
                throw new UserFriendlyException(
                    "Vui lòng chọn bảng giá.");
            }

            if (!input.ThoiGian.HasValue)
            {
                throw new UserFriendlyException(
                    "Vui lòng chọn ngày và giờ.");
            }

            // 2. Kiểm tra đăng nhập
            var userId = AbpSession.UserId;

            Console.WriteLine($"UserId: {userId}");

            if (!userId.HasValue)
            {
                throw new UserFriendlyException(
                    "Vui lòng đăng nhập để đặt lịch.");
            }

            // 3. Tìm khách hàng
            var khachHang = await _khachHangRepository
                .FirstOrDefaultAsync(x => x.UserId == userId.Value);

            Console.WriteLine(
                $"KhachHangId: {(khachHang != null ? khachHang.Id.ToString() : "NULL")}");

            if (khachHang == null)
            {
                throw new UserFriendlyException(
                    "Không tìm thấy thông tin khách hàng.");
            }

            int thuCungId = input.ThuCungId.Value;
            int dichVuId = input.DichVuId.Value;
            int bangGiaId = input.BangGiaId.Value;
            DateTime thoiGian = input.ThoiGian.Value;

            // 4. Kiểm tra thời gian
            if (thoiGian <= DateTime.Now)
            {
                throw new UserFriendlyException(
                    "Không thể đặt lịch ở thời gian đã qua.");
            }

            // 5. Kiểm tra thú cưng
            var thuCung = await _thuCungRepository
                .FirstOrDefaultAsync(x =>
                    x.Id == thuCungId &&
                    x.KhachHangId == khachHang.Id &&
                    x.TrangThai);

            Console.WriteLine(
                $"ThuCung: {(thuCung != null ? "OK" : "NULL")}");

            if (thuCung == null)
            {
                throw new UserFriendlyException(
                    "Thú cưng không thuộc tài khoản của bạn hoặc đã ngừng hoạt động.");
            }

            // 6. Kiểm tra dịch vụ
            var dichVu = await _dichVuRepository
                .FirstOrDefaultAsync(x =>
                    x.Id == dichVuId &&
                    x.TrangThai);

            Console.WriteLine(
                $"DichVu: {(dichVu != null ? "OK" : "NULL")}");

            if (dichVu == null)
            {
                throw new UserFriendlyException(
                    "Dịch vụ không tồn tại hoặc hiện không hoạt động.");
            }

            // 7. Kiểm tra bảng giá
            var bangGia = await _bangGiaRepository
                .FirstOrDefaultAsync(x =>
                    x.Id == bangGiaId &&
                    x.DichVuId == dichVuId);

            Console.WriteLine(
                $"BangGia: {(bangGia != null ? "OK" : "NULL")}");

            if (bangGia == null)
            {
                throw new UserFriendlyException(
                    "Bảng giá không thuộc dịch vụ đã chọn.");
            }

            // 8. Kiểm tra trùng lịch
            var lichTrung = await _lichChamSocRepository
                .GetAll()
                .AnyAsync(x =>
                    x.ThuCungId == thuCungId &&
                    x.ThoiGian == thoiGian &&
                    x.TrangThai != TrangThaiLichChamSoc.DaHuy);

            Console.WriteLine($"LichTrung: {lichTrung}");

            if (lichTrung)
            {
                throw new UserFriendlyException(
                    "Thú cưng đã có lịch chăm sóc vào thời gian này.");
            }

            // 9. Tạo lịch
            var lichChamSoc = new LichChamSoc
            {
                ThuCungId = thuCungId,
                DichVuId = dichVuId,
                BangGiaId = bangGiaId,
                KhachHangId = khachHang.Id,
                NhanVienId = null,
                ThoiGian = thoiGian,
                TrangThai = TrangThaiLichChamSoc.ChoXacNhan
            };

            Console.WriteLine("Đã tạo object LichChamSoc.");

            // 10. Insert
            var id = await _lichChamSocRepository
                .InsertAndGetIdAsync(lichChamSoc);

            Console.WriteLine($"Insert thành công. ID = {id}");

            await CurrentUnitOfWork.SaveChangesAsync();

            Console.WriteLine("SaveChanges thành công.");

            return id;
        }
        catch (UserFriendlyException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("========== CREATE ERROR ==========");
            Console.WriteLine(ex.ToString());

            throw new UserFriendlyException(
                "Có lỗi xảy ra khi tạo lịch: " + ex.Message);
        }
    }
    public async Task<List<LichChamSocDto>> GetLichChamSocCuaToi()
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
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : string.Empty,
                TrangThai = x.TrangThai
            })
            .ToListAsync();
    }

    public async Task<List<LichChamSocDto>> GetLichSuLichChamSocCuaToi()
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

        return await _lichChamSocRepository.GetAll()
            .Where(x => x.KhachHangId == khachHang.Id && x.TrangThai == TrangThaiLichChamSoc.HoanThanh)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                ThuCungId = x.ThuCungId,
                DichVuId = x.DichVuId,
                BangGiaId = x.BangGiaId,
                NhanVienId = x.NhanVienId,
                KhachHangId = x.KhachHangId,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .ToListAsync();
    }
}