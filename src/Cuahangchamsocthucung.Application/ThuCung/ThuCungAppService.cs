using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.ThuCung.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class ThuCungAppService : ApplicationService, IThuCungAppService
{
    private readonly IRepository<ThuCung, int> _thuCungRepository;
    private readonly IRepository<KhachHang, int> _khachHangRepository;
    private readonly IWebHostEnvironment _environment;


public ThuCungAppService(
    IRepository<ThuCung, int> thuCungRepository,
    IRepository<KhachHang, int> khachHangRepository,
    IWebHostEnvironment environment)
    {
        _thuCungRepository = thuCungRepository;
        _khachHangRepository = khachHangRepository;
        _environment = environment;
    }

    // =====================================================
    // LẤY TẤT CẢ THÚ CƯNG
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
    // LẤY THÚ CƯNG THEO ID
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
    // LẤY THÚ CƯNG CỦA KHÁCH HÀNG ĐANG ĐĂNG NHẬP
    // Chỉ lấy thú cưng đang hoạt động
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
    .Where(x => x.KhachHangId == khachHang.Id)
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
    // THÊM THÚ CƯNG
    // =====================================================

    public async Task<int> Create(ThemThuCungDto input)
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
        {
            throw new UserFriendlyException(
                "Vui lòng đăng nhập để thêm thú cưng.");
        }

        if (input == null)
        {
            throw new UserFriendlyException(
                "Dữ liệu thú cưng không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(input.TenThuCung))
        {
            throw new UserFriendlyException(
                "Tên thú cưng không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(input.LoaiThuCung))
        {
            throw new UserFriendlyException(
                "Vui lòng chọn loại thú cưng.");
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
            TenThuCung = input.TenThuCung.Trim(),
            LoaiThuCung = input.LoaiThuCung,
            GhiChu = string.IsNullOrWhiteSpace(input.GhiChu)
                ? null
                : input.GhiChu.Trim(),
            TrangThai = true,
            ImageUrl = input.ImageUrl
        };

        return await _thuCungRepository.InsertAndGetIdAsync(
            thuCung);
    }

    // =====================================================
    // UPLOAD / THAY ẢNH
    // =====================================================

    public async Task<string> UploadImage(
        int thuCungId,
        IFormFile file)
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
                x.Id == thuCungId &&
                x.KhachHangId == khachHang.Id);

        if (thuCung == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy thú cưng.");
        }

        if (file == null || file.Length == 0)
        {
            throw new UserFriendlyException(
                "Vui lòng chọn ảnh.");
        }

        var allowedExtensions = new[]
        {
        ".jpg",
        ".jpeg",
        ".png"
    };

        var extension = Path
            .GetExtension(file.FileName)
            .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new UserFriendlyException(
                "Chỉ chấp nhận ảnh PNG, JPG hoặc JPEG.");
        }

        if (file.ContentType != "image/jpeg" &&
            file.ContentType != "image/png")
        {
            throw new UserFriendlyException(
                "File không phải là ảnh PNG, JPG hoặc JPEG.");
        }

        var uploadPath = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "thucung",
            thuCungId.ToString());

        Directory.CreateDirectory(uploadPath);

        // Xóa ảnh cũ
        if (!string.IsNullOrEmpty(thuCung.ImageUrl))
        {
            var oldPath = Path.Combine(
                _environment.WebRootPath,
                thuCung.ImageUrl
                    .TrimStart('/')
                    .Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);
            }
        }

        var fileName =
            $"{Guid.NewGuid():N}{extension}";

        var filePath = Path.Combine(
            uploadPath,
            fileName);

        using (var stream = new FileStream(
            filePath,
            FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        thuCung.ImageUrl =
            $"/uploads/thucung/{thuCungId}/{fileName}";

        await _thuCungRepository.UpdateAsync(
            thuCung);

        await CurrentUnitOfWork.SaveChangesAsync();

        return thuCung.ImageUrl;
    }

    // =====================================================
    // CẬP NHẬT THÚ CƯNG
    // Không cập nhật ImageUrl ở đây.
    // Ảnh được xử lý riêng bằng UploadImage().
    // =====================================================

public async Task Update(SuaThuCungDto input)
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

        if (string.IsNullOrWhiteSpace(input.TenThuCung))
        {
            throw new UserFriendlyException(
                "Tên thú cưng không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(input.LoaiThuCung))
        {
            throw new UserFriendlyException(
                "Vui lòng chọn loại thú cưng.");
        }

        thuCung.TenThuCung = input.TenThuCung.Trim();
        thuCung.LoaiThuCung = input.LoaiThuCung;
        thuCung.GhiChu = input.GhiChu?.Trim();

        await _thuCungRepository.UpdateAsync(thuCung);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    // =====================================================
    // ĐỔI TRẠNG THÁI THÚ CƯNG
    // Thay cho Delete()
    // =====================================================

    public async Task ChangeStatus(int id)
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
        {
            throw new UserFriendlyException(
                "Vui lòng đăng nhập.");
        }

        var khachHang = await _khachHangRepository
            .FirstOrDefaultAsync(x =>
                x.UserId == userId.Value);

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

        thuCung.TrangThai =
            !thuCung.TrangThai;

        await _thuCungRepository.UpdateAsync(
            thuCung);

        await CurrentUnitOfWork.SaveChangesAsync();
    }


}
