using System.Linq;
using Cuahangchamsocthucung.Entities;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultThuCungCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultThuCungCreator(
            CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateThuCung();
        }

        private void CreateThuCung()
        {
            var khachHang = _context.KhachHangs
                .FirstOrDefault(x => x.SDT == "0912345678");

            if (khachHang == null)
            {
                return;
            }

            if (_context.ThuCungs.Any(
                x => x.KhachHangId == khachHang.Id))
            {
                return;
            }

            var thuCung = new ThuCung
            {
                KhachHangId = khachHang.Id,
                TenThuCung = "Lucky",
                LoaiThuCung = "Chó",
                GhiChu = "Chó Shiba",
                TrangThai = true,
                ImageUrl = "/img/thu-cung-duoi-10kg.jpg"
            };

            _context.ThuCungs.Add(thuCung);
            _context.SaveChanges();
        }
    }
}