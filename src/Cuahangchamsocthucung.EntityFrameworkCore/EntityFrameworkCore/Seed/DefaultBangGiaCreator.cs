using Cuahangchamsocthucung.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultBangGiaCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultBangGiaCreator(
            CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }


        public void Create()
        {
            if (_context.BangGias.Any())
            {
                return;
            }

            // Resolve DichVu ids by name to avoid hard-coded id assumptions
            var trongGiuDichVu = _context.DichVus.FirstOrDefault(d => d.TenDichVu == "Trông giữ thú cưng");
            var tamSpaDichVu = _context.DichVus.FirstOrDefault(d => d.TenDichVu == "Tắm Spa");
            var catTiaDichVu = _context.DichVus.FirstOrDefault(d => d.TenDichVu == "Cắt tỉa lông");

            if (trongGiuDichVu == null || tamSpaDichVu == null || catTiaDichVu == null)
            {
                // If related DichVus are not present, do not proceed to avoid FK issues.
                return;
            }

            var bangGias = new List<BangGia>
            {
                // Trông giữ thú cưng (Khách sạn)
                new BangGia(trongGiuDichVu.Id, 150000, "Chuồng M", 1, 10, false),
                new BangGia(trongGiuDichVu.Id, 200000, "Chuồng L", 1, 20, false),
                new BangGia(trongGiuDichVu.Id, 250000, "Chuồng XL", 1, 30, false),
                new BangGia(trongGiuDichVu.Id, 300000, "Phòng VIP C", 1, 10, false),
                new BangGia(trongGiuDichVu.Id, 400000, "Phòng VIP B", 1, 20, false),
                new BangGia(trongGiuDichVu.Id, 500000, "Phòng VIP A", 1, 40, false),

                // Tắm Spa / Cạo lông - Chó lông ngắn
                new BangGia(tamSpaDichVu.Id, 100000, "Chó", 1, 5, false),
                new BangGia(tamSpaDichVu.Id, 150000, "Chó", 5, 10, false),
                new BangGia(tamSpaDichVu.Id, 250000, "Chó", 10, 20, false),
                new BangGia(tamSpaDichVu.Id, 350000, "Chó", 20, 40, false),
                new BangGia(tamSpaDichVu.Id, 500000, "Chó", 40, 100, false),

                // Tắm Spa / Cạo lông - Chó lông dài
                new BangGia(tamSpaDichVu.Id, 150000, "Chó", 1, 5, true),
                new BangGia(tamSpaDichVu.Id, 250000, "Chó", 5, 10, true),
                new BangGia(tamSpaDichVu.Id, 350000, "Chó", 10, 20, true),
                new BangGia(tamSpaDichVu.Id, 500000, "Chó", 20, 40, true),
                new BangGia(tamSpaDichVu.Id, 650000, "Chó", 40, 100, true),

                // Tắm Spa - Mèo
                new BangGia(tamSpaDichVu.Id, 200000, "Mèo", 1, 5, false),
                new BangGia(tamSpaDichVu.Id, 300000, "Mèo", 5, 10, false),

                // Cắt / Cạo lông
                new BangGia(catTiaDichVu.Id, 250000, "Chó", 1, 2, false),
                new BangGia(catTiaDichVu.Id, 350000, "Chó", 2, 10, false),
                new BangGia(catTiaDichVu.Id, 500000, "Chó", 2, 10, false),
                new BangGia(catTiaDichVu.Id, 650000, "Chó", 2, 10, false)

            };

            _context.BangGias.AddRange(bangGias);
            _context.SaveChanges();
        }
    }
}