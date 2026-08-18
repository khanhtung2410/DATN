using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultKhachHangCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultKhachHangCreator(
            CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateKhachHang();
        }

        // Trong DefaultKhachHangCreator.cs
        private void CreateKhachHang()
        {
            // Đảm bảo tìm thấy User thuộc TenantId = 1
            var user = _context.Users
                .IgnoreQueryFilters() // Bỏ qua filter Tenant nếu cần
                .FirstOrDefault(x => x.UserName == "0912345678" && x.TenantId == 1);

            if (user == null) return;

            var existingKhachHang = _context.KhachHangs
                .IgnoreQueryFilters()
                .FirstOrDefault(x => x.UserId == user.Id);

            if (existingKhachHang != null) return;

            var khachHang = new KhachHang
            {
                TenantId = 1,
                UserId = user.Id,
                Hoten = "Nguyễn Văn Toàn",
                SDT = "0912345678",
                Email = "khachhang@gmail.com"
            };

            _context.KhachHangs.Add(khachHang);
            _context.SaveChanges();
        }
    }
}