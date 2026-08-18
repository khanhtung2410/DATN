using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Cuahangchamsocthucung.ThuCung.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;


public class ThuCungAppService :
ApplicationService,
IThuCungAppService
{
    private readonly IRepository<ThuCung, int> _thuCungRepository;
    private readonly IRepository<KhachHang, int> _khachHangRepository;

    public ThuCungAppService(
        IRepository<ThuCung, int> thuCungRepository,
        IRepository<KhachHang, int> khachHangRepository)
    {
        _thuCungRepository = thuCungRepository;
        _khachHangRepository = khachHangRepository;
    }


    // =====================================================
    // LẤY TẤT CẢ
    // =====================================================

    public async Task<List<ThuCungDto>> GetAll()
    {
        return await _thuCungRepository
            .GetAll()
            .Select(x => new ThuCungDto
            {
                Id = x.Id,
                KhachHangId = x.KhachHangId,
                TenThuCung = x.TenThuCung,
                LoaiThuCung = x.LoaiThuCung,
                GhiChu = x.GhiChu,
                TrangThai = x.TrangThai,
                ImageUrl = x.ImageUrl
            })
            .ToListAsync();
    }


    // =====================================================
    // LẤY THEO ID
    // =====================================================

    public async Task<ThuCungDto> Get(int id)
    {
        var thuCung = await _thuCungRepository
            .GetAll()
            .Where(x => x.Id == id)
            .Select(x => new ThuCungDto
            {
                Id = x.Id,
                KhachHangId = x.KhachHangId,
                TenThuCung = x.TenThuCung,
                LoaiThuCung = x.LoaiThuCung,
                GhiChu = x.GhiChu,
                TrangThai = x.TrangThai,
                ImageUrl = x.ImageUrl
            })
            .FirstOrDefaultAsync();

        if (thuCung == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thú cưng.");
        }

        return thuCung;
    }


    // =====================================================
    // LẤY THÚ CƯNG CỦA TÔI
    // =====================================================

    public async Task<List<ThuCungDto>> GetCuaToi()
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
        {
            throw new UserFriendlyException(
                "Vui lòng đăng nhập.");
        }

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.UserId == userId.Value);

        if (khachHang == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thông tin khách hàng.");
        }

        return await _thuCungRepository
            .GetAll()
            .Where(x =>
                x.KhachHangId == khachHang.Id &&
                x.TrangThai)
            .Select(x => new ThuCungDto
            {
                Id = x.Id,
                KhachHangId = x.KhachHangId,
                TenThuCung = x.TenThuCung,
                LoaiThuCung = x.LoaiThuCung,
                GhiChu = x.GhiChu,
                TrangThai = x.TrangThai,
                ImageUrl = x.ImageUrl
            })
            .ToListAsync();
    }


    // =====================================================
    // THÊM
    // =====================================================

    public async Task<int> Create(ThemThuCungDto input)
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
        {
            throw new UserFriendlyException(
                "Vui lòng đăng nhập để thêm thú cưng.");
        }

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.UserId == userId.Value);

        if (khachHang == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thông tin khách hàng.");
        }

        var thuCung = new ThuCung
        {
            KhachHangId = khachHang.Id,

            TenThuCung = input.TenThuCung,

            LoaiThuCung = input.LoaiThuCung,

            GhiChu = input.GhiChu,

            TrangThai = input.TrangThai,

            ImageUrl = input.ImageUrl
        };

        return await _thuCungRepository
            .InsertAndGetIdAsync(thuCung);
    }


    // =====================================================
    // SỬA
    // =====================================================

    public async Task Update(SuaThuCungDto input)
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
        {
            throw new UserFriendlyException(
                "Vui lòng đăng nhập.");
        }

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.UserId == userId.Value);

        if (khachHang == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thông tin khách hàng.");
        }

        var thuCung = await _thuCungRepository
            .FirstOrDefaultAsync(x =>
                x.Id == input.Id &&
                x.KhachHangId == khachHang.Id);

        if (thuCung == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thú cưng.");
        }

        thuCung.TenThuCung = input.TenThuCung;

        thuCung.LoaiThuCung = input.LoaiThuCung;

        thuCung.GhiChu = input.GhiChu;

        thuCung.TrangThai = input.TrangThai;

        thuCung.ImageUrl = input.ImageUrl;

        await _thuCungRepository.UpdateAsync(thuCung);
    }


    // =====================================================
    // XÓA
    // =====================================================

    public async Task Delete(int id)
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
        {
            throw new UserFriendlyException(
                "Vui lòng đăng nhập.");
        }

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x => x.UserId == userId.Value);

        if (khachHang == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thông tin khách hàng.");
        }

        var thuCung = await _thuCungRepository
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.KhachHangId == khachHang.Id);

        if (thuCung == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thú cưng.");
        }

        await _thuCungRepository.DeleteAsync(thuCung);
    }
}

