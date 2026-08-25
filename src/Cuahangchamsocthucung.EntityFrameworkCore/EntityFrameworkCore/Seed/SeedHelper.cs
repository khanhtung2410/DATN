using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.EntityFrameworkCore.Seed.Host;
using Cuahangchamsocthucung.EntityFrameworkCore.Seed.Tenants;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<CuahangchamsocthucungDbContext>(
                iocResolver,
                context =>
                {
                    SeedHostDb(context, iocResolver);
                });
        }

        public static void SeedHostDb(
            CuahangchamsocthucungDbContext context,
            IIocResolver iocResolver)
        {
            context.SuppressAutoSetTenantId = true;

            // Host
            new InitialHostDbBuilder(context).Create();

            // Tenant
            new DefaultTenantBuilder(context).Create();

            // Role + User mặc định
            new TenantRoleAndUserBuilder(
                context,
                1
            ).Create();

            // Dữ liệu Tenant
            new DefaultTenantDataCreator(
                context,
                1
            ).Create();

            // Dịch vụ
            new DefaultDichVuCreator(context).Create();

            // Bảng giá
            new DefaultBangGiaCreator(context).Create();

            // Mặt hàng
            new DefaultMatHangCreator(context).Create();

            // Nhân viên
            new DefaultNhanVienCreator(context).Create();

            // User khách hàng
            using (var userManager =
                iocResolver.ResolveAsDisposable<UserManager>())
            using (var uowManager =
                iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                new DefaultUserCreator(
                    context,
                    userManager.Object,
                    uowManager.Object
                ).Create();
            }
            // VIP
            new DefaultVipCreator(context, 1).Create();
            // Khách hàng
            new DefaultKhachHangCreator(context).Create();
            // Thú cưng
            new DefaultThuCungCreator(context).Create();
            // Lịch chăm sóc
            new DefaultLichChamSocCreator(context).Create();
            // Hóa đơn
            new DefaultHoaDonCreator(context).Create();
        }

        private static void WithDbContext<TDbContext>(
            IIocResolver iocResolver,
            Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager =
                iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow =
                    uowManager.Object.Begin(
                        TransactionScopeOption.Suppress))
                {
                    var context =
                        uowManager.Object
                            .Current
                            .GetDbContext<TDbContext>(
                                MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }
    }
}