using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using Cuahangchamsocthucung.Authorization;

namespace Cuahangchamsocthucung.Web.Startup
{
    public class CuahangchamsocthucungNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(new MenuItemDefinition(
                    PageNames.Home, L("HomePage"), url: "", icon: "fas fa-home", requiresAuthentication: true))
                //.AddItem(new MenuItemDefinition(
                //    PageNames.Tenants, L("Tenants"), url: "Tenants", icon: "fas fa-building", permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Tenants)))
                //.AddItem(new MenuItemDefinition(
                //    PageNames.Users, L("Users"), url: "Users", icon: "fas fa-users", permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Users)))
                .AddItem(new MenuItemDefinition(
                    PageNames.Roles, L("Roles"), url: "Roles", icon: "fas fa-theater-masks", permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Roles)))
                .AddItem(new MenuItemDefinition(
                    PageNames.HoaDon, new FixedLocalizableString("Hóa Đơn"), url: "HoaDon", icon: "fas fa-file-invoice"))
                .AddItem(new MenuItemDefinition(
                    PageNames.KhachHang, new FixedLocalizableString("Khách Hàng"), url: "KhachHang", icon: "fas fa-user-friends"))
                .AddItem(new MenuItemDefinition(
                    PageNames.DichVu, new FixedLocalizableString("Dịch Vụ"), url: "DichVu", icon: "fas fa-concierge-bell"))
              .AddItem(new MenuItemDefinition(
    PageNames.LichChamSoc,
    new FixedLocalizableString("Lịch Chăm Sóc"),
    url: null,
    icon: "fas fa-calendar-alt")
    .AddItem(new MenuItemDefinition(
        "LichChamSocIndex",
        new FixedLocalizableString("Danh sách"),
        url: "LichChamSoc",
        icon: "fas fa-list"
    ))
    .AddItem(new MenuItemDefinition(
        PageNames.LichChamSocTimeline,
        new FixedLocalizableString("Timeline"),
        url: "LichChamSoc/Timeline",
        icon: "fas fa-stream"
    ))
)
                .AddItem(new MenuItemDefinition(
                    PageNames.NhanVien, new FixedLocalizableString("Nhân Viên"), url: "NhanVien", icon: "fas fa-id-badge"))
                .AddItem(new MenuItemDefinition(
                    PageNames.MatHang, new FixedLocalizableString("Mặt Hàng"), url: "MatHang", icon: "fas fa-box-open"))
                .AddItem(new MenuItemDefinition(
                    PageNames.Vip, new FixedLocalizableString("VIP"), url: "Vip", icon: "fas fa-crown"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, CuahangchamsocthucungConsts.LocalizationSourceName);
        }
    }
}