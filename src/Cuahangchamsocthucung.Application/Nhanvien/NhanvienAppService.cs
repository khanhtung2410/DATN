using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Cuahangchamsocthucung.DichVu.Dto;
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
        IRepository<NhanVien> nhanVienRepository
       )
    {
        _nhanVienRepository = nhanVienRepository;
    }

    public async Task ChangeTrangThai(SuaTrangThaiNhanVienDto input)
    {
        var nhanVien = await _nhanVienRepository.GetAsync(input.Id);
        nhanVien.Trangthai = input.Trangthai;
        await _nhanVienRepository.UpdateAsync(nhanVien);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task<int> Create(ThemNhanVienDto input)
    {
        if (input.Hoten == null || input.SDT == null || input.Gioitinh == null || input.Ngayvaolam == null)
        {
            throw new UserFriendlyException("Thông tin bắt buộc không được để trống.");
        }
        if (string.IsNullOrWhiteSpace(input.SDT) ||
     input.SDT.Length != 10 ||
     !input.SDT.All(char.IsDigit) ||
     !(input.SDT.StartsWith("03") ||
       input.SDT.StartsWith("05") ||
       input.SDT.StartsWith("07") ||
       input.SDT.StartsWith("08") ||
       input.SDT.StartsWith("09")))
        {
            throw new UserFriendlyException("Số điện thoại không hợp lệ.");
        }
        if (_nhanVienRepository.GetAll().Any(x => x.SDT == input.SDT))
        {
            throw new UserFriendlyException("Số điện thoại đã tồn tại.");
        }
        var nhanVien = new NhanVien
        {
            Hoten = input.Hoten,
            Gioitinh = input.Gioitinh,
            Ngaysinh = input.Ngaysinh,
            Ngayvaolam = input.Ngayvaolam,
            SDT = input.SDT,
            Trangthai = true
        };
        var nhanVienId = await _nhanVienRepository.InsertAndGetIdAsync(nhanVien);
        await CurrentUnitOfWork.SaveChangesAsync();
        return nhanVienId;
    }

    public Task<List<NhanVienDto>> GetAll()
    {
        var query = _nhanVienRepository.GetAll();
            return Task.FromResult(query.Select(x => new NhanVienDto
            {
            Id = x.Id,
            Hoten = x.Hoten,
            Gioitinh = x.Gioitinh,
            Ngaysinh = x.Ngaysinh,
            Ngayvaolam = x.Ngayvaolam,
            SDT = x.SDT,
            Trangthai = x.Trangthai
        }).ToList());
    }

    public Task<NhanVienDto> GetNhanVien(int id)
    {
       var nhanVien = _nhanVienRepository.GetAll().FirstOrDefault(x => x.Id == id);
        if (nhanVien == null)
        {
            throw new UserFriendlyException("Nhân viên không tồn tại.");
        }
        var NhanVienDto = new NhanVienDto
        {
            Id = nhanVien.Id,
            Hoten = nhanVien.Hoten,
            Gioitinh = nhanVien.Gioitinh,
            Ngaysinh = nhanVien.Ngaysinh,
            Ngayvaolam = nhanVien.Ngayvaolam,
            SDT = nhanVien.SDT,
            Trangthai = nhanVien.Trangthai
        };
        return Task.FromResult(NhanVienDto);
    }

    public async Task Update(SuaNhanVienDto input)
    {
       var nhanVien = _nhanVienRepository.GetAll().FirstOrDefault(x => x.Id == input.Id);
        if (nhanVien == null)
        {
            throw new UserFriendlyException("Nhân viên không tồn tại.");
        }
        if(input.Hoten == null || input.SDT == null|| input.Gioitinh == null || input.Ngayvaolam == null)
        {
            throw new UserFriendlyException("Thông tin bắt buộc không được để trống.");
        }
        if (string.IsNullOrWhiteSpace(input.SDT) ||
     input.SDT.Length != 10 ||
     !input.SDT.All(char.IsDigit) ||
     !(input.SDT.StartsWith("03") ||
       input.SDT.StartsWith("05") ||
       input.SDT.StartsWith("07") ||
       input.SDT.StartsWith("08") ||
       input.SDT.StartsWith("09")))
        {
            throw new UserFriendlyException("Số điện thoại không hợp lệ.");
        }
        if(input.SDT != nhanVien.SDT)
        {
            var existingNhanVien = _nhanVienRepository.GetAll().FirstOrDefault(x => x.SDT == input.SDT);
            if (existingNhanVien != null)
            {
                throw new UserFriendlyException("Số điện thoại đã tồn tại.");
            }
        }
        //Update thông tin nhân viên
        nhanVien.Hoten = input.Hoten;
        nhanVien.Gioitinh = input.Gioitinh;
        nhanVien.Ngaysinh = input.Ngaysinh;
        nhanVien.Ngayvaolam = input.Ngayvaolam;
        nhanVien.SDT = input.SDT;
        nhanVien.Trangthai = input.Trangthai;
         await _nhanVienRepository.UpdateAsync(nhanVien);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
}