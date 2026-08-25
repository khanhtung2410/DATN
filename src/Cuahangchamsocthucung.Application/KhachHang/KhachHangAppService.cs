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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class KhachHangAppService : ApplicationService, IKhachHangAppService
{
    private const int RegistrationTenantId = 1;
    private readonly IRepository<KhachHang, int> _khachHangRepository;
    private readonly UserManager<User> _userManager;
    private readonly IRepository<ThuCung, int> _thuCungRepository;
    private readonly IRepository<UserRole, long> _userRoleRepository;
    private readonly IRepository<Role, int> _roleRepository;
    private readonly IRepository<HoaDon, int> _hoaDonRepository;
    private readonly IRepository<Vip, int> _vipRepository;
    private readonly IRepository<CauHinhVip, int> _cauHinhVipRepository;


public KhachHangAppService(
    IRepository<KhachHang, int> khachHangRepository,
    UserManager<User> userManager,
    IRepository<ThuCung, int> thuCungRepository,
    IRepository<UserRole, long> userRoleRepository,
    IRepository<Role, int> roleRepository,
    IRepository<HoaDon, int> hoaDonRepository,
    IRepository<Vip, int> vipRepository,
    IRepository<CauHinhVip, int> cauHinhVipRepository)
    {
        _khachHangRepository = khachHangRepository;
        _userManager = userManager;
        _thuCungRepository = thuCungRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _hoaDonRepository = hoaDonRepository;
        _vipRepository = vipRepository;
        _cauHinhVipRepository = cauHinhVipRepository;
    }

    public async Task<KhachHangDto> DangKy(DangKyDto input)
    {
        if (input == null)
            throw new UserFriendlyException("Thông tin đăng ký không hợp lệ.");
        if (string.IsNullOrWhiteSpace(input.HoTen))
            throw new UserFriendlyException("Họ và tên không được để trống.");
        if (string.IsNullOrWhiteSpace(input.SDT))
            throw new UserFriendlyException("Số điện thoại không được để trống.");
        if (string.IsNullOrWhiteSpace(input.MatKhau))
            throw new UserFriendlyException("Mật khẩu không được để trống.");
        if (input.MatKhau != input.XacNhanMatKhau)
            throw new UserFriendlyException("Mật khẩu xác nhận không khớp.");

        var existedKhachHang = await _khachHangRepository.GetAll().AnyAsync(x => x.SDT == input.SDT);
        if (existedKhachHang)
            throw new UserFriendlyException("Số điện thoại đã được đăng ký.");

        var existedUser = await _userManager.Users.IgnoreQueryFilters().AnyAsync(x =>
            x.TenantId == RegistrationTenantId && x.UserName == input.SDT);

        if (existedUser)
            throw new UserFriendlyException("Số điện thoại đã được sử dụng.");

        var emailAddress = string.IsNullOrWhiteSpace(input.Email)
            ? $"{input.SDT}@noemail.local"
            : input.Email.Trim();

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

        var createResult = await _userManager.CreateAsync(user, input.MatKhau);
        if (!createResult.Succeeded)
            throw new UserFriendlyException(string.Join(", ", createResult.Errors.Select(x => x.Description)));

        var customerRole = await _roleRepository.GetAll()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>
                x.TenantId == RegistrationTenantId &&
                x.Name == StaticRoleNames.Tenants.Customer);

        if (customerRole == null)
            throw new UserFriendlyException($"Không tìm thấy role Customer cho Tenant {RegistrationTenantId}.");

        var existingUserRole = await _userRoleRepository.GetAll()
            .IgnoreQueryFilters()
            .AnyAsync(x =>
                x.TenantId == RegistrationTenantId &&
                x.UserId == user.Id &&
                x.RoleId == customerRole.Id);

        if (!existingUserRole)
            await _userRoleRepository.InsertAsync(new UserRole(RegistrationTenantId, user.Id, customerRole.Id));

        var khachHang = new KhachHang
        {
            TenantId = RegistrationTenantId,
            UserId = user.Id,
            Hoten = input.HoTen.Trim(),
            SDT = input.SDT.Trim(),
            Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim()
        };

        await _khachHangRepository.InsertAsync(khachHang);
        await CurrentUnitOfWork.SaveChangesAsync();

        return new KhachHangDto
        {
            Id = khachHang.Id,
            Hoten = khachHang.Hoten,
            SDT = khachHang.SDT,
            Email = khachHang.Email,
            TrangThai = user.IsActive
        };
    }

    public async Task<List<KhachHangDto>> GetAllKhachHangAsync()
    {
        var khachHangs = await _khachHangRepository.GetAll()
            .Select(x => new
            {
                KhachHang = x,
                UserIsActive = x.User != null && x.User.IsActive,
                Vip = x.Vip
            })
            .ToListAsync();

        var khachHangIds = khachHangs.Select(x => x.KhachHang.Id).ToList();

        if (!khachHangIds.Any())
            return new List<KhachHangDto>();

        var hoaDons = await _hoaDonRepository.GetAll()
            .Where(x => khachHangIds.Contains(x.KhachHangId) && x.TrangThai == "DaThanhToan")
            .Select(x => new
            {
                x.KhachHangId,
                x.TongTien
            })
            .ToListAsync();

        var vipIds = khachHangs
            .Where(x => x.Vip != null)
            .Select(x => x.Vip.Id)
            .Distinct()
            .ToList();

        var cauHinhVips = await _cauHinhVipRepository.GetAll()
            .Where(x => vipIds.Contains(x.VipId))
            .OrderByDescending(x => x.TuNgay)
            .ToListAsync();

        var allVips = await _vipRepository.GetAll()
            .OrderBy(x => x.CapVip)
            .ToListAsync();

        var allCauHinhVips = await _cauHinhVipRepository.GetAll()
            .OrderBy(x => x.MucChiTieu)
            .ToListAsync();

        var thuCungs = await _thuCungRepository.GetAll()
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

        var result = new List<KhachHangDto>();

        foreach (var item in khachHangs)
        {
            var khachHang = item.KhachHang;
            var tongChiTieu = hoaDons
                .Where(x => x.KhachHangId == khachHang.Id)
                .Sum(x => x.TongTien);

            var vipHienTai = item.Vip;
            var cauHinhHienTai = vipHienTai == null
                ? null
                : allCauHinhVips
                    .Where(x => x.VipId == vipHienTai.Id)
                    .OrderByDescending(x => x.TuNgay)
                    .FirstOrDefault();

            var vipTiepTheo = vipHienTai == null
                ? allVips.FirstOrDefault()
                : allVips.FirstOrDefault(x => x.CapVip > vipHienTai.CapVip);

            var cauHinhTiepTheo = vipTiepTheo == null
                ? null
                : allCauHinhVips
                    .Where(x => x.VipId == vipTiepTheo.Id)
                    .OrderBy(x => x.MucChiTieu)
                    .FirstOrDefault();

            var mucChiTieuVip = cauHinhHienTai?.MucChiTieu ?? 0;
            var mucChiTieuVipTiepTheo = cauHinhTiepTheo?.MucChiTieu ?? 0;
            var conThieuVip = vipTiepTheo == null
                ? 0
                : System.Math.Max(0, mucChiTieuVipTiepTheo - tongChiTieu);

            result.Add(new KhachHangDto
            {
                Id = khachHang.Id,
                Hoten = khachHang.Hoten,
                SDT = khachHang.SDT,
                Email = khachHang.Email,
                TrangThai = item.UserIsActive,
                VipId = vipHienTai?.Id,
                TenVip = vipHienTai?.TenVip,
                CapVip = vipHienTai?.CapVip ?? 0,
                MucChiTieuVip = mucChiTieuVip,
                TongChiTieu = tongChiTieu,
                TenVipTiepTheo = vipTiepTheo?.TenVip,
                MucChiTieuVipTiepTheo = mucChiTieuVipTiepTheo,
                ConThieuVip = conThieuVip,
                ThuCungs = thuCungs.Where(x => x.KhachHangId == khachHang.Id).ToList()
            });
        }

        return result;
    }

    public async Task<KhachHangDto> GetKhachHangByIdAsync(int id)
    {
        var khachHang = await _khachHangRepository.GetAll()
            .Include(x => x.User)
            .Include(x => x.Vip)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (khachHang == null)
            throw new UserFriendlyException("Không tìm thấy khách hàng.");

        var tongChiTieu = await _hoaDonRepository.GetAll()
            .Where(x => x.KhachHangId == id && x.TrangThai == "DaThanhToan")
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var allVips = await _vipRepository.GetAll()
            .OrderBy(x => x.CapVip)
            .ToListAsync();

        var allCauHinhVips = await _cauHinhVipRepository.GetAll()
            .OrderBy(x => x.MucChiTieu)
            .ToListAsync();

        var cauHinhHienTai = khachHang.Vip == null
            ? null
            : allCauHinhVips
                .Where(x => x.VipId == khachHang.Vip.Id)
                .OrderByDescending(x => x.TuNgay)
                .FirstOrDefault();

        var vipTiepTheo = khachHang.Vip == null
            ? allVips.FirstOrDefault()
            : allVips.FirstOrDefault(x => x.CapVip > khachHang.Vip.CapVip);

        var cauHinhTiepTheo = vipTiepTheo == null
            ? null
            : allCauHinhVips
                .Where(x => x.VipId == vipTiepTheo.Id)
                .OrderBy(x => x.MucChiTieu)
                .FirstOrDefault();

        var thuCungs = await _thuCungRepository.GetAll()
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

        var mucChiTieuVip = cauHinhHienTai?.MucChiTieu ?? 0;
        var mucChiTieuVipTiepTheo = cauHinhTiepTheo?.MucChiTieu ?? 0;

        return new KhachHangDto
        {
            Id = khachHang.Id,
            Hoten = khachHang.Hoten,
            SDT = khachHang.SDT,
            Email = khachHang.Email,
            TrangThai = khachHang.User?.IsActive ?? false,
            VipId = khachHang.Vip?.Id,
            TenVip = khachHang.Vip?.TenVip,
            CapVip = khachHang.Vip?.CapVip ?? 0,
            MucChiTieuVip = mucChiTieuVip,
            TongChiTieu = tongChiTieu,
            TenVipTiepTheo = vipTiepTheo?.TenVip,
            MucChiTieuVipTiepTheo = mucChiTieuVipTiepTheo,
            ConThieuVip = vipTiepTheo == null ? 0 : System.Math.Max(0, mucChiTieuVipTiepTheo - tongChiTieu),
            ThuCungs = thuCungs
        };
    }

    public async Task<KhachHangDto> GetThongTinCaNhanAsync()
    {
        if (!AbpSession.UserId.HasValue)
            throw new UserFriendlyException("Bạn chưa đăng nhập.");

        var khachHang = await _khachHangRepository.GetAll()
            .Include(x => x.User)
            .Include(x => x.Vip)
            .FirstOrDefaultAsync(x => x.UserId == AbpSession.UserId.Value);

        if (khachHang == null)
            throw new UserFriendlyException("Không tìm thấy thông tin khách hàng.");

        var tongChiTieu = await _hoaDonRepository.GetAll()
            .Where(x => x.KhachHangId == khachHang.Id && x.TrangThai == "DaThanhToan")
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var allVips = await _vipRepository.GetAll()
            .OrderBy(x => x.CapVip)
            .ToListAsync();

        var allCauHinhVips = await _cauHinhVipRepository.GetAll()
            .OrderBy(x => x.MucChiTieu)
            .ToListAsync();

        var vipHienTai = khachHang.Vip;

        var cauHinhHienTai = vipHienTai == null
            ? null
            : allCauHinhVips
                .Where(x => x.VipId == vipHienTai.Id)
                .OrderByDescending(x => x.TuNgay)
                .FirstOrDefault();

        var vipTiepTheo = vipHienTai == null
            ? allVips.FirstOrDefault()
            : allVips.FirstOrDefault(x => x.CapVip > vipHienTai.CapVip);

        var cauHinhTiepTheo = vipTiepTheo == null
            ? null
            : allCauHinhVips
                .Where(x => x.VipId == vipTiepTheo.Id)
                .OrderByDescending(x => x.TuNgay)
                .FirstOrDefault();

        var mucChiTieuVip = cauHinhHienTai?.MucChiTieu ?? 0;
        var mucChiTieuVipTiepTheo = cauHinhTiepTheo?.MucChiTieu ?? 0;
        var conThieuVip = vipTiepTheo == null
            ? 0
            : Math.Max(0, mucChiTieuVipTiepTheo - tongChiTieu);

        return new KhachHangDto
        {
            Id = khachHang.Id,
            Hoten = khachHang.Hoten,
            SDT = khachHang.SDT,
            Email = khachHang.Email,
            TrangThai = khachHang.User?.IsActive ?? false,
            VipId = vipHienTai?.Id,
            TenVip = vipHienTai?.TenVip,
            CapVip = vipHienTai?.CapVip ?? 0,
            PhanTramGiam = cauHinhHienTai?.PhanTramGiam ?? 0,
            MucChiTieuVip = mucChiTieuVip,
            TongChiTieu = tongChiTieu,
            TenVipTiepTheo = vipTiepTheo?.TenVip,
            PhanTramGiamTiepTheo = cauHinhTiepTheo?.PhanTramGiam ?? 0,
            MucChiTieuVipTiepTheo = mucChiTieuVipTiepTheo,
            ConThieuVip = conThieuVip
        };
    }
}
