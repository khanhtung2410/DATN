using System.Collections.Generic;
using System.Linq;
using Cuahangchamsocthucung.Entities;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultNhanVienCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultNhanVienCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            if (_context.NhanViens.Any())
            {
                return;
            }

            var nhanViens = new List<NhanVien>
            {
                new NhanVien("Nguyễn Văn Toàn", "0912345678", true, new System.DateOnly(1990,1,15), new System.DateOnly(2020,1,1), true),
                new NhanVien("Trần Thị Như", "0922345678", false, new System.DateOnly(1992,3,20), new System.DateOnly(2021,5,10), true),
                new NhanVien("Lê Văn Dương", "0932345678", true, new System.DateOnly(1988,7,5), new System.DateOnly(2019,9,1), true),
                new NhanVien("Phạm Thị Nha", "0942345678", false, new System.DateOnly(1995,11,30), new System.DateOnly(2022,2,15), true)
            };

            _context.NhanViens.AddRange(nhanViens);
            _context.SaveChanges();
        }
    }
}