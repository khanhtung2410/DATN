using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultVipCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;
        private readonly int _tenantId;

        public DefaultVipCreator(CuahangchamsocthucungDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            var vips = new[]
            {
                new { CapVip = 0, TenVip = "Thường", MucChiTieu = 0m, PhanTramGiam = 0m },
                new { CapVip = 1, TenVip = "VIP 1", MucChiTieu = 1000000m, PhanTramGiam = 2m },
                new { CapVip = 2, TenVip = "VIP 2", MucChiTieu = 3000000m, PhanTramGiam = 5m },
                new { CapVip = 3, TenVip = "VIP 3", MucChiTieu = 5000000m, PhanTramGiam = 10m },
                new { CapVip = 4, TenVip = "VIP 4", MucChiTieu = 10000000m, PhanTramGiam = 15m },
                new { CapVip = 5, TenVip = "VIP 5", MucChiTieu = 20000000m, PhanTramGiam = 20m }
            };

            foreach (var item in vips)
            {
                var vip = _context.Vips
                    .IgnoreQueryFilters()
                    .Include(x => x.CauHinhVips)
                    .FirstOrDefault(x => x.TenantId == _tenantId && x.CapVip == item.CapVip);

                if (vip == null)
                {
                    vip = new Vip
                    {
                        TenantId = _tenantId,
                        CapVip = item.CapVip,
                        TenVip = item.TenVip
                    };

                    _context.Vips.Add(vip);
                    _context.SaveChanges();
                }

                var cauHinh = _context.CauHinhVips
                    .IgnoreQueryFilters()
                    .FirstOrDefault(x => x.TenantId == _tenantId && x.VipId == vip.Id);

                if (cauHinh == null)
                {
                    cauHinh = new CauHinhVip
                    {
                        TenantId = _tenantId,
                        VipId = vip.Id,
                        MucChiTieu = item.MucChiTieu,
                        PhanTramGiam = item.PhanTramGiam,
                        TuNgay = DateTime.Today,
                        DenNgay = null
                    };

                    _context.CauHinhVips.Add(cauHinh);
                }
                else
                {
                    cauHinh.MucChiTieu = item.MucChiTieu;
                    cauHinh.PhanTramGiam = item.PhanTramGiam;
                    cauHinh.TuNgay = DateTime.Today;
                    cauHinh.DenNgay = null;

                    _context.CauHinhVips.Update(cauHinh);
                }
            }

            _context.SaveChanges();
        }
    }
}