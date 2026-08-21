using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultKhachHangCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultKhachHangCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateKhachHang();
        }

        private void CreateKhachHang()
        {
            var data = new[]
            {
                new { Sdt = "0912345678", HoTen = "Nguyễn Văn Toàn", Email = "khachhang01@gmail.com" },
                new { Sdt = "0912345679", HoTen = "Trần Thị Lan", Email = "khachhang02@gmail.com" },
                new { Sdt = "0912345680", HoTen = "Lê Hoàng Nam", Email = "khachhang03@gmail.com" },
                new { Sdt = "0912345681", HoTen = "Phạm Minh Anh", Email = "khachhang04@gmail.com" },
                new { Sdt = "0912345682", HoTen = "Vũ Đức Minh", Email = "khachhang05@gmail.com" },
                new { Sdt = "0912345683", HoTen = "Đặng Thu Hà", Email = "khachhang06@gmail.com" },
                new { Sdt = "0912345684", HoTen = "Bùi Quang Huy", Email = "khachhang07@gmail.com" },
                new { Sdt = "0912345685", HoTen = "Đỗ Ngọc Mai", Email = "khachhang08@gmail.com" },
                new { Sdt = "0912345686", HoTen = "Hồ Gia Bảo", Email = "khachhang09@gmail.com" },
                new { Sdt = "0912345687", HoTen = "Ngô Phương Thảo", Email = "khachhang10@gmail.com" },
                new { Sdt = "0912345688", HoTen = "Dương Tuấn Anh", Email = "khachhang11@gmail.com" },
                new { Sdt = "0912345689", HoTen = "Mai Khánh Linh", Email = "khachhang12@gmail.com" },
                new { Sdt = "0912345690", HoTen = "Phan Hữu Phước", Email = "khachhang13@gmail.com" },
                new { Sdt = "0912345691", HoTen = "Tạ Thanh Tâm", Email = "khachhang14@gmail.com" },
                new { Sdt = "0912345692", HoTen = "Cao Minh Khang", Email = "khachhang15@gmail.com" }
            };

            foreach (var item in data)
            {
                var user = _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefault(x => x.UserName == item.Sdt && x.TenantId == 1);

                if (user == null)
                    continue;

                var existing = _context.KhachHangs
                    .IgnoreQueryFilters()
                    .FirstOrDefault(x => x.UserId == user.Id);

                if (existing != null)
                    continue;

                _context.KhachHangs.Add(new KhachHang
                {
                    TenantId = 1,
                    UserId = user.Id,
                    Hoten = item.HoTen,
                    SDT = item.Sdt,
                    Email = item.Email
                });
            }

            _context.SaveChanges();
        }
    }
}