using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Cuahangchamsocthucung.Authorization
{
    public class CuahangchamsocthucungAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission(PermissionNames.Pages_LichChamSoc, new LocalizableString(
        "Lịch chăm sóc",
        CuahangchamsocthucungConsts.LocalizationSourceName
    ));
            context.CreatePermission(PermissionNames.Pages_DichVu, L("Dịch vụ"));
            context.CreatePermission(PermissionNames.Pages_NhanVien, L("Nhân viên"));
            context.CreatePermission(PermissionNames.Pages_HoaDon, L("Hóa đơn"));
            context.CreatePermission(PermissionNames.Pages_KhachHang, L("Khách hàng"));
            context.CreatePermission(PermissionNames.Pages_Vip, L("VIP"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, CuahangchamsocthucungConsts.LocalizationSourceName);
        }
    }
}
