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
                // Dịch vụ 1: Trông giữ thú cưng (Khách sạn)
                // ===========================
                new BangGia(1, 150000, "Chuồng M", 1, 10, false),    // ≤ 10kg
                new BangGia(1, 200000, "Chuồng L", 1, 20, false),    // ≤ 20kg
                new BangGia(1, 250000, "Chuồng XL", 1, 30, false),   // ≤ 30kg
                new BangGia(1, 300000, "Phòng VIP C", 1, 10, false), // ≤ 10kg
                new BangGia(1, 400000, "Phòng VIP B", 1, 20, false), // ≤ 20kg
                new BangGia(1, 500000, "Phòng VIP A", 1, 40, false), // ≤ 40kg

                // ===========================
                // Dịch vụ 2: Tắm Spa / Cạo lông
                // Bảng: cân nặng vs giá (lông ngắn / lông dài)
                // ===========================
                // Lông ngắn
                new BangGia(2, 100000, "Chó", 1, 5, false),    // < 5kg
                new BangGia(2, 150000, "Chó", 5, 10, false),   // 5–10kg
                new BangGia(2, 250000, "Chó", 10, 20, false),  // 10–20kg
                new BangGia(2, 350000, "Chó", 20, 40, false),  // 20–40kg
                new BangGia(2, 500000, "Chó", 40, 100, false), // > 40kg (upper bound large)

                // Lông dài
                new BangGia(2, 150000, "Chó", 1, 5, true),     // < 5kg
                new BangGia(2, 250000, "Chó", 5, 10, true),    // 5–10kg
                new BangGia(2, 350000, "Chó", 10, 20, true),   // 10–20kg
                new BangGia(2, 500000, "Chó", 20, 40, true),   // 20–40kg
                new BangGia(2, 650000, "Chó", 40, 100, true),  // > 40kg

                // Mèo (tối thiểu mẫu)
                new BangGia(2, 200000, "Mèo", 1, 5, false),
                new BangGia(2, 300000, "Mèo", 5, 10, false),

                // ===========================
                // Dịch vụ 3: Cắt / Cạo lông (giá mẫu theo bảng ngắn)
                // ===========================
                new BangGia(3, 200000, "Chó", 1, 5, false),   // < 5kg
                new BangGia(3, 300000, "Chó", 5, 10, false),  // 5–10kg

                // (Bạn có thể mở rộng thêm các mức cân nặng hoặc cho lông dài ở đây nếu cần)
            };

            _context.BangGias.AddRange(bangGias);
            _context.SaveChanges();
        }
    }
}