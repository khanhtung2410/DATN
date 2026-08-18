using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Authorization.Users;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly CuahangchamsocthucungDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(
            CuahangchamsocthucungDbContext context,
            int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            // =====================================================
            // 1. Admin role
            // =====================================================

            var adminRole = _context.Roles
                .IgnoreQueryFilters()
                .FirstOrDefault(r =>
                    r.TenantId == _tenantId &&
                    r.Name == StaticRoleNames.Tenants.Admin);

            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(
                    new Role(
                        _tenantId,
                        StaticRoleNames.Tenants.Admin,
                        StaticRoleNames.Tenants.Admin)
                    {
                        IsStatic = true,
                        NormalizedName = StaticRoleNames.Tenants.Admin.ToUpperInvariant()
                    }).Entity;

                _context.SaveChanges();
            }
            else if (string.IsNullOrEmpty(adminRole.NormalizedName))
            {
                // Bổ sung NormalizedName nếu DB cũ bị NULL
                adminRole.NormalizedName = StaticRoleNames.Tenants.Admin.ToUpperInvariant();
                _context.SaveChanges();
            }

            // =====================================================
            // 2. Customer role
            // =====================================================

            var customerRole = _context.Roles
                .IgnoreQueryFilters()
                .FirstOrDefault(r =>
                    r.TenantId == _tenantId &&
                    r.Name == StaticRoleNames.Tenants.Customer);

            if (customerRole == null)
            {
                customerRole = _context.Roles.Add(
                    new Role(
                        _tenantId,
                        StaticRoleNames.Tenants.Customer,
                        "Khách hàng")
                    {
                        IsStatic = true,
                        IsDefault = false,
                        NormalizedName = StaticRoleNames.Tenants.Customer.ToUpperInvariant()
                    }).Entity;

                _context.SaveChanges();
            }
            else if (string.IsNullOrEmpty(customerRole.NormalizedName))
            {
                // Bổ sung NormalizedName nếu DB cũ bị NULL
                customerRole.NormalizedName = StaticRoleNames.Tenants.Customer.ToUpperInvariant();
                _context.SaveChanges();
            }

            // =====================================================
            // 3. Grant permissions to Admin
            // =====================================================

            var grantedPermissions = _context.Permissions
                .IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p =>
                    p.TenantId == _tenantId &&
                    p.RoleId == adminRole.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(
                    new CuahangchamsocthucungAuthorizationProvider())
                .Where(p =>
                    p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                    !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission =>
                        new RolePermissionSetting
                        {
                            TenantId = _tenantId,
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = adminRole.Id
                        })
                );

                _context.SaveChanges();
            }

            // =====================================================
            // 4. Admin user
            // =====================================================

            var adminUser = _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefault(u =>
                    u.TenantId == _tenantId &&
                    u.UserName == AbpUserBase.AdminUserName);

            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(
                    _tenantId,
                    "admin@defaulttenant.com");

                adminUser.NormalizedUserName = AbpUserBase.AdminUserName.ToUpperInvariant();
                adminUser.NormalizedEmailAddress = adminUser.EmailAddress.ToUpperInvariant();

                adminUser.Password =
                    new PasswordHasher<User>(
                        new OptionsWrapper<PasswordHasherOptions>(
                            new PasswordHasherOptions()))
                    .HashPassword(
                        adminUser,
                        "123qwe");

                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin role
                _context.UserRoles.Add(
                    new UserRole(
                        _tenantId,
                        adminUser.Id,
                        adminRole.Id));

                _context.SaveChanges();
            }
            else
            {
                // Tự động fix NormalizedUserName/NormalizedEmailAddress nếu User đã tồn tại trong DB
                bool isUserUpdated = false;

                if (string.IsNullOrEmpty(adminUser.NormalizedUserName))
                {
                    adminUser.NormalizedUserName = adminUser.UserName.ToUpperInvariant();
                    isUserUpdated = true;
                }

                if (string.IsNullOrEmpty(adminUser.NormalizedEmailAddress) && !string.IsNullOrEmpty(adminUser.EmailAddress))
                {
                    adminUser.NormalizedEmailAddress = adminUser.EmailAddress.ToUpperInvariant();
                    isUserUpdated = true;
                }

                if (isUserUpdated)
                {
                    _context.SaveChanges();
                }

                // Kiểm tra và gán Role nếu bị thiếu gán trong AbpUserRoles
                var hasAdminRole = _context.UserRoles
                    .IgnoreQueryFilters()
                    .Any(ur => ur.TenantId == _tenantId && ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);

                if (!hasAdminRole)
                {
                    _context.UserRoles.Add(
                        new UserRole(
                            _tenantId,
                            adminUser.Id,
                            adminRole.Id));

                    _context.SaveChanges();
                }
            }
        }
    }
}