using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.NhanVien.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class NhanvienAppService :
    ApplicationService,
    INhanvienAppService
{
    private readonly IRepository<NhanVien> _nhanVienRepository;

    public NhanvienAppService(
        IRepository<NhanVien> nhanVienRepository)
    {
        _nhanVienRepository = nhanVienRepository;
    }

    // Đổi trạng thái nhân viên
    public async Task ChangeTrangThai(SuaTrangThaiNhanVienDto input)
    {
        var nhanVien = await _nhanVienRepository.GetAsync(input.Id);

        nhanVien.Trangthai = input.Trangthai;

        await _nhanVienRepository.UpdateAsync(nhanVien);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    // Thêm nhân viên
    public async Task<int> Create(ThemNhanVienDto input)
    {
        if (input == null)
        {
            throw new UserFriendlyException(
                "Thông tin nhân viên không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(input.Hoten))
        {
            throw new UserFriendlyException(
                "Họ tên không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(input.SDT))
        {
            throw new UserFriendlyException(
                "Số điện thoại không được để trống.");
        }

        if (input.Ngayvaolam == default)
        {
            throw new UserFriendlyException(
                "Ngày vào làm không được để trống.");
        }

        if (input.Luong < 0)
        {
            throw new UserFriendlyException(
                "Lương không được nhỏ hơn 0.");
        }

        // Kiểm tra số điện thoại
        if (input.SDT.Length != 10 ||
            !input.SDT.All(char.IsDigit) ||
            !(input.SDT.StartsWith("03") ||
              input.SDT.StartsWith("05") ||
              input.SDT.StartsWith("07") ||
              input.SDT.StartsWith("08") ||
              input.SDT.StartsWith("09")))
        {
            throw new UserFriendlyException(
                "Số điện thoại không hợp lệ.");
        }

        // Kiểm tra trùng số điện thoại
        var exists = await _nhanVienRepository
            .GetAll()
            .AnyAsync(x => x.SDT == input.SDT);

        if (exists)
        {
            throw new UserFriendlyException(
                "Số điện thoại đã tồn tại.");
        }

        var nhanVien = new NhanVien
        {
            Hoten = input.Hoten.Trim(),
            Gioitinh = input.Gioitinh,
            Ngaysinh = input.Ngaysinh,
            Ngayvaolam = input.Ngayvaolam,
            SDT = input.SDT,
            Luong = input.Luong,
            Trangthai = true
        };

        var nhanVienId =
            await _nhanVienRepository.InsertAndGetIdAsync(nhanVien);

        await CurrentUnitOfWork.SaveChangesAsync();

        return nhanVienId;
    }

    // Lấy tất cả nhân viên
    public async Task<List<NhanVienDto>> GetAll()
    {
        return await _nhanVienRepository
            .GetAll()
            .Select(x => new NhanVienDto
            {
                Id = x.Id,
                Hoten = x.Hoten,
                Gioitinh = x.Gioitinh,
                Ngaysinh = x.Ngaysinh,
                Ngayvaolam = x.Ngayvaolam,
                SDT = x.SDT,
                Luong = x.Luong,
                Trangthai = x.Trangthai
            })
            .ToListAsync();
    }

    // Lấy thông tin một nhân viên
    public async Task<NhanVienDto> GetNhanVien(int id)
    {
        var nhanVien = await _nhanVienRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (nhanVien == null)
        {
            throw new UserFriendlyException(
                "Nhân viên không tồn tại.");
        }

        return new NhanVienDto
        {
            Id = nhanVien.Id,
            Hoten = nhanVien.Hoten,
            Gioitinh = nhanVien.Gioitinh,
            Ngaysinh = nhanVien.Ngaysinh,
            Ngayvaolam = nhanVien.Ngayvaolam,
            SDT = nhanVien.SDT,
            Luong = nhanVien.Luong,
            Trangthai = nhanVien.Trangthai
        };
    }

    // Cập nhật nhân viên
    public async Task Update(SuaNhanVienDto input)
    {
        if (input == null)
        {
            throw new UserFriendlyException(
                "Thông tin nhân viên không hợp lệ.");
        }

        var nhanVien = await _nhanVienRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == input.Id);

        if (nhanVien == null)
        {
            throw new UserFriendlyException(
                "Nhân viên không tồn tại.");
        }

        if (string.IsNullOrWhiteSpace(input.Hoten))
        {
            throw new UserFriendlyException(
                "Họ tên không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(input.SDT))
        {
            throw new UserFriendlyException(
                "Số điện thoại không được để trống.");
        }

        if (input.Ngayvaolam == default)
        {
            throw new UserFriendlyException(
                "Ngày vào làm không được để trống.");
        }

        if (input.Luong < 0)
        {
            throw new UserFriendlyException(
                "Lương không được nhỏ hơn 0.");
        }

        // Kiểm tra số điện thoại
        if (input.SDT.Length != 10 ||
            !input.SDT.All(char.IsDigit) ||
            !(input.SDT.StartsWith("03") ||
              input.SDT.StartsWith("05") ||
              input.SDT.StartsWith("07") ||
              input.SDT.StartsWith("08") ||
              input.SDT.StartsWith("09")))
        {
            throw new UserFriendlyException(
                "Số điện thoại không hợp lệ.");
        }

        // Nếu thay đổi số điện thoại thì kiểm tra trùng
        if (input.SDT != nhanVien.SDT)
        {
            var existingNhanVien = await _nhanVienRepository
                .GetAll()
                .FirstOrDefaultAsync(x =>
                    x.SDT == input.SDT &&
                    x.Id != input.Id);

            if (existingNhanVien != null)
            {
                throw new UserFriendlyException(
                    "Số điện thoại đã tồn tại.");
            }
        }

        // Cập nhật thông tin
        nhanVien.Hoten = input.Hoten.Trim();
        nhanVien.Gioitinh = input.Gioitinh;
        nhanVien.Ngaysinh = input.Ngaysinh;
        nhanVien.Ngayvaolam = input.Ngayvaolam;
        nhanVien.SDT = input.SDT;
        nhanVien.Luong = input.Luong;
        nhanVien.Trangthai = input.Trangthai;

        await _nhanVienRepository.UpdateAsync(nhanVien);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    // Lấy nhân viên đang làm việc
    public async Task<List<NhanVienDto>> GetNhanVienDangLamViec()
    {
        return await _nhanVienRepository
            .GetAll()
            .Where(x => x.Trangthai)
            .Select(x => new NhanVienDto
            {
                Id = x.Id,
                Hoten = x.Hoten,
                Gioitinh = x.Gioitinh,
                Ngaysinh = x.Ngaysinh,
                Ngayvaolam = x.Ngayvaolam,
                SDT = x.SDT,
                Luong = x.Luong,
                Trangthai = x.Trangthai
            })
            .ToListAsync();
    }

    // Tổng chi phí lương nhân viên đang làm việc
    public async Task<decimal> GetTongChiPhiLuong()
    {
        return await _nhanVienRepository
            .GetAll()
            .Where(x => x.Trangthai)
            .SumAsync(x => x.Luong);
    }
}