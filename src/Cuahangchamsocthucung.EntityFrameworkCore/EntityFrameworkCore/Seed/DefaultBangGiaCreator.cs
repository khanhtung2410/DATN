using Cuahangchamsocthucung.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultBangGiaCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultBangGiaCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            if (_context.BangGias.Any()) return;

            var trongGiuDichVu = _context.DichVus.FirstOrDefault(d => d.TenDichVu == "Trông giữ thú cưng");
            var tamSpaDichVu = _context.DichVus.FirstOrDefault(d => d.TenDichVu == "Tắm Spa");
            var catTiaDichVu = _context.DichVus.FirstOrDefault(d => d.TenDichVu == "Cắt tỉa lông");

            if (trongGiuDichVu == null || tamSpaDichVu == null || catTiaDichVu == null) return;

            var bangGias = new List<BangGia>
            {
                // TRÔNG GIỮ - CHÓ
                new BangGia(trongGiuDichVu.Id, 150000, "Chó", 1, 10, false, 1440, "Chuồng M"),
                new BangGia(trongGiuDichVu.Id, 200000, "Chó", 1, 20, false, 1440, "Chuồng L"),
                new BangGia(trongGiuDichVu.Id, 250000, "Chó", 1, 30, false, 1440, "Chuồng XL"),
                new BangGia(trongGiuDichVu.Id, 300000, "Chó", 1, 10, false, 1440, "Phòng VIP C"),
                new BangGia(trongGiuDichVu.Id, 400000, "Chó", 1, 20, false, 1440, "Phòng VIP B"),
                new BangGia(trongGiuDichVu.Id, 500000, "Chó", 1, 40, false, 1440, "Phòng VIP A"),

                // TRÔNG GIỮ - MÈO
                new BangGia(trongGiuDichVu.Id, 150000, "Mèo", 1, 10, false, 1440, "Chuồng M"),
                new BangGia(trongGiuDichVu.Id, 300000, "Mèo", 1, 10, false, 1440, "Phòng VIP C"),

                // TẮM SPA - CHÓ LÔNG NGẮN
                new BangGia(tamSpaDichVu.Id, 100000, "Chó", 1, 5, false, 60),
                new BangGia(tamSpaDichVu.Id, 150000, "Chó", 5, 10, false, 75),
                new BangGia(tamSpaDichVu.Id, 250000, "Chó", 10, 20, false, 90),
                new BangGia(tamSpaDichVu.Id, 350000, "Chó", 20, 40, false, 120),
                new BangGia(tamSpaDichVu.Id, 500000, "Chó", 40, 100, false, 150),

                // TẮM SPA - CHÓ LÔNG DÀI
                new BangGia(tamSpaDichVu.Id, 150000, "Chó", 1, 5, true, 75),
                new BangGia(tamSpaDichVu.Id, 250000, "Chó", 5, 10, true, 90),
                new BangGia(tamSpaDichVu.Id, 350000, "Chó", 10, 20, true, 120),
                new BangGia(tamSpaDichVu.Id, 500000, "Chó", 20, 40, true, 150),
                new BangGia(tamSpaDichVu.Id, 650000, "Chó", 40, 100, true, 180),

                // TẮM SPA - MÈO
                new BangGia(tamSpaDichVu.Id, 200000, "Mèo", 1, 5, false, 60),
                new BangGia(tamSpaDichVu.Id, 300000, "Mèo", 5, 10, false, 90),

                // CẮT TỈA - CHÓ
                new BangGia(catTiaDichVu.Id, 250000, "Chó", 1, 2, false, 60),
                new BangGia(catTiaDichVu.Id, 350000, "Chó", 2, 10, false, 90),
                new BangGia(catTiaDichVu.Id, 500000, "Chó", 10, 20, false, 120),
                new BangGia(catTiaDichVu.Id, 650000, "Chó", 20, 40, false, 150),

                // CẮT TỈA - MÈO
                new BangGia(catTiaDichVu.Id, 250000, "Mèo", 1, 2, false, 60),
                new BangGia(catTiaDichVu.Id, 350000, "Mèo", 2, 10, false, 90),
                new BangGia(catTiaDichVu.Id, 500000, "Mèo", 10, 20, false, 120),
                new BangGia(catTiaDichVu.Id, 650000, "Mèo", 20, 40, false, 150)
            };

            _context.BangGias.AddRange(bangGias);
            _context.SaveChanges();
        }
    }
}