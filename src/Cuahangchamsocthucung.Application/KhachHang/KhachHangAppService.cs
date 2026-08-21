using Abp.Application.Services;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.UI;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.KhachHang.Dto;
using Cuahangchamsocthucung.ThuCung.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class KhachHangAppService :
    ApplicationService,
    IKhachHangAppService
{
    private const int RegistrationTenantId = 1;

    private readonly IRepository<KhachHang, int> _khachHangRepository;
    private readonly UserRegistrationManager _userRegistrationManager;
    private readonly UserManager<User> _userManager;
    private readonly IRepository<ThuCung, int> _thuCungRepository;
    private readonly IRepository<UserRole, long> _userRoleRepository;
    private readonly IRepository<Role, int> _roleRepository;

    public KhachHangAppService(
        IRepository<KhachHang, int> khachHangRepository,
        UserRegistrationManager userRegistrationManager,
        UserManager<User> userManager,
        IRepository<ThuCung, int> thuCungRepository,
        IRepository<UserRole, long> userRoleRepository,
        IRepository<Role, int> roleRepository)
    {
        _khachHangRepository = khachHangRepository;
        _userRegistrationManager = userRegistrationManager;
        _userManager = userManager;
        _thuCungRepository = thuCungRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    public async Task<KhachHangDto> DangKy(DangKyDto input)
    {
        // =====================================================
        // 1. Kiểm tra dữ liệu đầu vào
        // =====================================================

        if (input == null)
        {
            throw new UserFriendlyException(
                "Thông tin đăng ký không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(input.HoTen))
        {
            throw new UserFriendlyException(
                "Họ và tên không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(input.SDT))
        {
            throw new UserFriendlyException(
                "Số điện thoại không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(input.MatKhau))
        {
            throw new UserFriendlyException(
                "Mật khẩu không được để trống.");
        }

        if (input.MatKhau != input.XacNhanMatKhau)
        {
            throw new UserFriendlyException(
                "Mật khẩu xác nhận không khớp.");
        }

        // =====================================================
        // 2. Kiểm tra số điện thoại trong bảng KhachHang
        // =====================================================

        var existedKhachHang = await _khachHangRepository
            .GetAll()
            .AnyAsync(x => x.SDT == input.SDT);

        if (existedKhachHang)
        {
            throw new UserFriendlyException(
                "Số điện thoại đã được đăng ký.");
        }

        // =====================================================
        // 3. Kiểm tra User trong đúng Tenant
        // =====================================================

        var existedUser = await _userManager
            .Users
            .IgnoreQueryFilters()
            .AnyAsync(x =>
                x.TenantId == RegistrationTenantId &&
                x.UserName == input.SDT);

        if (existedUser)
        {
            throw new UserFriendlyException(
                "Số điện thoại đã được sử dụng.");
        }

        // =====================================================
        // 4. Email
        // ABP yêu cầu EmailAddress không được null khi CreateAsync
        // nhưng Email đăng ký của khách hàng vẫn không bắt buộc.
        // =====================================================

        var emailAddress = string.IsNullOrWhiteSpace(input.Email)
            ? $"{input.SDT}@noemail.local"
            : input.Email.Trim();

        // =====================================================
        // 5. Tạo User
        // =====================================================

        var user = new User
        {
            TenantId = RegistrationTenantId,
            UserName = input.SDT,
            Name = input.HoTen.Trim(),
            Surname = "",
            EmailAddress = emailAddress,
            IsActive = true,
            IsEmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(
            user,
            input.MatKhau);

        if (!createResult.Succeeded)
        {
            throw new UserFriendlyException(
                string.Join(
                    ", ",
                    createResult.Errors.Select(x => x.Description)));
        }

        // =====================================================
        // 6. Tìm Role Customer của Tenant 1
        // =====================================================

        var customerRole = await _roleRepository
            .GetAll()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>
                x.TenantId == RegistrationTenantId &&
                x.Name == StaticRoleNames.Tenants.Customer);

        if (customerRole == null)
        {
            throw new UserFriendlyException(
                $"Không tìm thấy role Customer cho Tenant {RegistrationTenantId}.");
        }

        // =====================================================
        // 7. Gán User vào Role Customer
        // Không dùng AddToRoleAsync vì lúc đăng ký anonymous
        // ABP không xác định được Tenant hiện tại.
        // =====================================================

        var existingUserRole = await _userRoleRepository
            .GetAll()
            .IgnoreQueryFilters()
            .AnyAsync(x =>
                x.TenantId == RegistrationTenantId &&
                x.UserId == user.Id &&
                x.RoleId == customerRole.Id);

        if (!existingUserRole)
        {
            await _userRoleRepository.InsertAsync(
                new UserRole(
                    RegistrationTenantId,
                    user.Id,
                    customerRole.Id));
        }

        // =====================================================
        // 8. Tạo KhachHang
        // Email thật có thể null
        // =====================================================

        var khachHang = new KhachHang
        {
            TenantId = RegistrationTenantId,
            UserId = user.Id,
            Hoten = input.HoTen.Trim(),
            SDT = input.SDT.Trim(),
            Email = string.IsNullOrWhiteSpace(input.Email)
                ? null
                : input.Email.Trim()
        };

        await _khachHangRepository.InsertAsync(khachHang);

        // =====================================================
        // 9. Lưu toàn bộ
        // =====================================================

        await CurrentUnitOfWork.SaveChangesAsync();

        // =====================================================
        // 10. Trả kết quả
        // =====================================================

        return new KhachHangDto
        {
            Id = khachHang.Id,
            Hoten = khachHang.Hoten,
            SDT = khachHang.SDT,
            Email = khachHang.Email
        };
    }

    public async Task<List<KhachHangDto>> GetAllKhachHangAsync()
    {
        var khachHangs = await _khachHangRepository
            .GetAll()
            .Select(x => new KhachHangDto
            {
                Id = x.Id,
                Hoten = x.Hoten,
                SDT = x.SDT,
                Email = x.Email
            })
            .ToListAsync();

        var khachHangIds = khachHangs
            .Select(x => x.Id)
            .ToList();

        var thuCungs = await _thuCungRepository
            .GetAll()
            .Where(x => khachHangIds.Contains(x.KhachHangId))
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

        foreach (var khachHang in khachHangs)
        {
            khachHang.ThuCungs = thuCungs
                .Where(x => x.KhachHangId == khachHang.Id)
                .ToList();
        }

        return khachHangs;
    }

    public async Task<KhachHangDto> GetKhachHangByIdAsync(int id)
    {
        var khachHang = await _khachHangRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (khachHang == null)
        {
            throw new UserFriendlyException(
                "Không tìm thấy khách hàng.");
        }

        var thuCungs = await _thuCungRepository
            .GetAll()
            .Where(x => x.KhachHangId == id)
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

        return new KhachHangDto
        {
            Id = khachHang.Id,
            Hoten = khachHang.Hoten,
            SDT = khachHang.SDT,
            Email = khachHang.Email,
            ThuCungs = thuCungs
        };
    }
}
