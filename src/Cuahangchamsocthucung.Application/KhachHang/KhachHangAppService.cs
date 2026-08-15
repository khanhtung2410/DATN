using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.KhachHang.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class KhachHangAppService :
    ApplicationService,
    IKhachHangAppService
{
    private readonly IRepository<KhachHang, int> _khachHangRepository;
    private readonly UserRegistrationManager _userRegistrationManager;
    private readonly UserManager<User> _userManager;    

    public KhachHangAppService(
        IRepository<KhachHang, int> khachHangRepository,
        UserRegistrationManager userRegistrationManager,
        UserManager<User> userManager)
    {
        _khachHangRepository = khachHangRepository;
        _userRegistrationManager = userRegistrationManager;
        _userManager = userManager;
    }

    public async Task<KhachHangDto> DangKy(DangKyDto input)
    {
        // 1. Kiểm tra số điện thoại đã tồn tại trong KhachHang
        var existedKhachHang = await _khachHangRepository
            .GetAll()
            .AnyAsync(x => x.SDT == input.SDT);

        if (existedKhachHang)
        {
            throw new UserFriendlyException(
                "Số điện thoại đã được đăng ký.");
        }

        // 2. Kiểm tra mật khẩu xác nhận
        if (input.MatKhau != input.XacNhanMatKhau)
        {
            throw new UserFriendlyException(
                "Mật khẩu xác nhận không khớp.");
        }

        // 3. Kiểm tra UserName (SDT) đã tồn tại trong User
        var existedUser = await _userManager
            .Users
            .AnyAsync(x => x.UserName == input.SDT);

        if (existedUser)
        {
            throw new UserFriendlyException(
                "Số điện thoại đã được sử dụng.");
        }

        // 4. Tạo User
        var user = new User
        {
            UserName = input.SDT,
            Name = input.HoTen,
            Surname = "",
            EmailAddress = input.Email,
            IsActive = true,
            IsEmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(
            user,
            input.MatKhau
        );

        // 5. Kiểm tra tạo User
        if (!createResult.Succeeded)
        {
            throw new UserFriendlyException(
                string.Join(
                    ", ",
                    createResult.Errors.Select(x => x.Description)
                )
            );
        }

        // 6. Gán role Customer
        var roleResult = await _userManager.AddToRoleAsync(
            user,
           StaticRoleNames.Tenants.Customer
        );

        if (!roleResult.Succeeded)
        {
            throw new UserFriendlyException(
                string.Join(
                    ", ",
                    roleResult.Errors.Select(x => x.Description)
                )
            );
        }

        // 7. Tạo KhachHang
        var khachHang = new KhachHang
        {
            UserId = user.Id,
            Hoten = input.HoTen,
            SDT = input.SDT,
            Email = input.Email
        };

        await _khachHangRepository.InsertAsync(khachHang);

        await CurrentUnitOfWork.SaveChangesAsync();

        // 8. Trả kết quả
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
        return await _khachHangRepository
            .GetAll()
            .Select(x => new KhachHangDto
            {
                Id = x.Id,
                Hoten = x.Hoten,
                SDT = x.SDT,
                Email = x.Email
            })
            .ToListAsync();
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

        return new KhachHangDto
        {
            Id = khachHang.Id,
            Hoten = khachHang.Hoten,
            SDT = khachHang.SDT,
            Email = khachHang.Email
        };
    }
}