using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.LichChamSoc.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LichChamSocAppService :
    ApplicationService,
    ILichChamSocAppService
{
    private readonly IRepository<LichChamSoc> _lichChamSocRepository;
    private readonly IRepository<KhachHang> _khachHangRepository;

    public LichChamSocAppService(
        IRepository<LichChamSoc> lichChamSocRepository,
        IRepository<KhachHang> khachHangRepository
       )
    {
        _lichChamSocRepository = lichChamSocRepository;
        _khachHangRepository = khachHangRepository;
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
        var lichChamSoc = new LichChamSoc
        {
            DichVuId = input.DichVuId,
            KhachHangId = input.KhachHangId,
            ThoiGian = input.ThoiGian,
            TrangThai = TrangThaiLichChamSoc.ChoXacNhan
        };

        await _lichChamSocRepository.InsertAndGetIdAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
        return lichChamSoc.Id;
    }

    public async Task<List<LichChamSocDto>> GetLichChamSocCuaToi()
    {
        var userId = AbpSession.UserId;

        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(x => x.UserId == userId);

        if (khachHang == null)
        {
            throw new UserFriendlyException("Không tìm thấy khách hàng.");
        }

        return await _lichChamSocRepository.GetAll()
            .Where(x => x.KhachHangId == khachHang.Id)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                DichVuId = x.DichVuId,
                NhanVienId = x.NhanVienId,
                KhachHangId = x.KhachHangId,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .ToListAsync();
    }
    public async Task<List<LichChamSocDto>> GetLichSuLichChamSocCuaToi()
    {
        var userId = AbpSession.UserId;
        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(x => x.UserId == userId);
        if (khachHang == null)
        {
            throw new UserFriendlyException("Không tìm thấy khách hàng.");
        }
        return await _lichChamSocRepository.GetAll()
            .Where(x => x.KhachHangId == khachHang.Id && x.TrangThai == TrangThaiLichChamSoc.HoanThanh)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                DichVuId = x.DichVuId,
                NhanVienId = x.NhanVienId,
                KhachHangId = x.KhachHangId,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .ToListAsync();
    }
}