using System.Linq;
using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultTenantDataCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;
        private readonly int _tenantId;

        public DefaultTenantDataCreator(CuahangchamsocthucungDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateKhachHangsForTenant();
        }

        private void CreateKhachHangsForTenant()
        {
            // If there are already customers for this tenant, skip.
            if (_context.KhachHangs.IgnoreQueryFilters().Any(k => k.TenantId == _tenantId))
            {
                return;
            }

            // Find some users that belong to the tenant to associate with KhachHang.UserId.
            var users = _context.Users.IgnoreQueryFilters()
                                      .Where(u => u.TenantId == _tenantId)
                                      .Take(3)
                                      .ToList();

            if (!users.Any())
            {
                // No tenant users available yet; skip seeding customers.
                return;
            }

            var khachHangs = users.Select((u, index) => new KhachHang
            {
                TenantId = _tenantId,
                UserId = u.Id,
                Hoten = $"{u.Name} { (index==0 ? "Khách" : $"KH{index}") }",
                SDT = GenerateUniqueSdt(index),
                Email = u.EmailAddress
            }).ToList();

            _context.KhachHangs.AddRange(khachHangs);
            _context.SaveChanges();
        }

        private string GenerateUniqueSdt(int index)
        {
            // Ensure 10-digit VN-like phone numbers and uniqueness per seeded index
            // Example: 0900000001, 0900000002, ...
            return $"090000000{index + 1}";
        }
    }
}