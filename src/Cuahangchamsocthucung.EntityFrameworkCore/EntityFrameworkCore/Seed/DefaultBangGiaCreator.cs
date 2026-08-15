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

            var bangGias = new List<BangGia>
{
    // ===========================
    // Dịch vụ 1: Trông giữ thú cưng
    // ===========================

    new BangGia(1, 150000, "Chó/Mèo - Chuồng M", 1, 10, false),
    new BangGia(1, 200000, "Chó - Chuồng L", 1, 20, false),
    new BangGia(1, 250000, "Chó - Chuồng XL", 1, 30, false),
    new BangGia(1, 300000, "Chó/Mèo - Phòng VIP C", 1, 10, false),
    new BangGia(1, 400000, "Chó/Mèo - Phòng VIP B", 1, 20, false),
    new BangGia(1, 500000, "Chó/Mèo - Phòng VIP A", 1, 40, false),

    // ===========================
    // Dịch vụ 2: Tắm Spa
    // ===========================

    // Chó lông ngắn
    new BangGia(2, 100000, "Chó", 1, 5, false),
    new BangGia(2, 150000, "Chó", 5, 10, false),
    new BangGia(2, 250000, "Chó", 10, 20, false),
    new BangGia(2, 350000, "Chó", 20, 40, false),
    new BangGia(2, 500000, "Chó", 40, 100, false),

    // Chó lông dài
    new BangGia(2, 150000, "Chó", 1, 5, true),
    new BangGia(2, 250000, "Chó", 5, 10, true),
    new BangGia(2, 350000, "Chó", 10, 20, true),
    new BangGia(2, 500000, "Chó", 20, 40, true),
    new BangGia(2, 650000, "Chó", 40, 100, true),

    // Mèo
    new BangGia(2, 200000, "Mèo", 1, 5, false),
    new BangGia(2, 300000, "Mèo", 5, 10, false),

    // ===========================
    // Dịch vụ 3: Cắt tỉa lông
    // ===========================

    // Chó lông ngắn
    new BangGia(3, 200000, "Chó", 1, 5, false),
    new BangGia(3, 300000, "Chó", 5, 10, false),
    new BangGia(3, 450000, "Chó", 10, 20, false),
    new BangGia(3, 600000, "Chó", 20, 40, false),

    // Chó lông dài
    new BangGia(3, 300000, "Chó", 1, 5, true),
    new BangGia(3, 450000, "Chó", 5, 10, true),
    new BangGia(3, 600000, "Chó", 10, 20, true),
    new BangGia(3, 800000, "Chó", 20, 40, true),

    // Mèo
    new BangGia(3, 250000, "Mèo", 1, 5, false),
    new BangGia(3, 350000, "Mèo", 5, 10, false)
};

            _context.BangGias.AddRange(bangGias);
            _context.SaveChanges();


            _context.SaveChanges();
        }
    }
}