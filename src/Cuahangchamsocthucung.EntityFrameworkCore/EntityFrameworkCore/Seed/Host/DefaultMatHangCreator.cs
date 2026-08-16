using System.Collections.Generic;
using System.Linq;
using Cuahangchamsocthucung.Entities;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultMatHangCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultMatHangCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            if (_context.MatHangs.Any())
            {
                return;
            }

            var matHangs = new List<MatHang>
            {
                new MatHang { Tenmathang = "Sữa tắm chó - 250ml", Mota = "Sữa tắm dịu nhẹ, ngăn rụng lông", Soluong = 40, Trangthai = true },
                new MatHang { Tenmathang = "Sữa tắm chó - 500ml", Mota = "Sữa tắm dưỡng ẩm cho chó mọi lứa tuổi", Soluong = 25, Trangthai = true },
                new MatHang { Tenmathang = "Xả lông (Conditioner) - 250ml", Mota = "Dưỡng mềm lông sau khi tắm", Soluong = 30, Trangthai = true },
                new MatHang { Tenmathang = "Máy sấy lông - 2000W", Mota = "Máy sấy chuyên dụng cho thú cưng", Soluong = 8, Trangthai = true },
                new MatHang { Tenmathang = "Thức ăn khô cho chó - 5kg", Mota = "Thức ăn cân bằng dinh dưỡng cho chó trưởng thành", Soluong = 15, Trangthai = true },
                new MatHang { Tenmathang = "Thức ăn khô cho mèo - 2kg", Mota = "Công thức cho mèo mọi độ tuổi", Soluong = 20, Trangthai = true },
                new MatHang { Tenmathang = "Chuồng size S", Mota = "Chuồng nhựa nhỏ cho chó, mèo nhỏ", Soluong = 6, Trangthai = true },
                new MatHang { Tenmathang = "Chuồng chó size M", Mota = "Chuồng hợp lý cho chó vừa", Soluong = 6, Trangthai = true },
                new MatHang { Tenmathang = "Chuồng chó size L", Mota = "Chuồng chắc chắn cho chó lớn", Soluong = 4, Trangthai = true },
                new MatHang { Tenmathang = "Chuồng chó size XL", Mota = "Chuồng lớn cho chó to/đực", Soluong = 2, Trangthai = true },
                new MatHang { Tenmathang = "Bấm móng (Nail Clipper)", Mota = "Bấm móng thép không gỉ cho chó/mèo", Soluong = 35, Trangthai = true },
                new MatHang { Tenmathang = "Kéo tỉa lông", Mota = "Kéo tỉa chuyên dụng cho groomer", Soluong = 18, Trangthai = true },
                new MatHang { Tenmathang = "Máy cắt lông (Clipper)", Mota = "Máy cắt lông cầm tay, nhiều lược dẫn hướng", Soluong = 10, Trangthai = true },
                new MatHang { Tenmathang = "Ngoáy tai - Dung dịch", Mota = "Dung dịch vệ sinh tai cho chó mèo", Soluong = 40, Trangthai = true },
                new MatHang { Tenmathang = "Nước diệt bọ (Anti-flea) - 250ml", Mota = "Thuốc xịt diệt bọ ve cho thú cưng", Soluong = 22, Trangthai = true },
                new MatHang { Tenmathang = "Xịt dưỡng lông - 150ml", Mota = "Xịt tạo mượt, khử mùi hôi", Soluong = 28, Trangthai = true },
                new MatHang { Tenmathang = "Lược chải lông - Thường", Mota = "Lược chải hàng ngày cho chó/mèo", Soluong = 45, Trangthai = true },
                new MatHang { Tenmathang = "Lược chải lông - Răng dày", Mota = "Lược cho lông dày, lông rối", Soluong = 25, Trangthai = true },
                new MatHang { Tenmathang = "Dầu dưỡng da & lông", Mota = "Dưỡng da khô, bật sáng lông", Soluong = 12, Trangthai = true },
                new MatHang { Tenmathang = "Bộ chăm sóc móng (kit)", Mota = "Gồm bấm móng, dũa, kềm", Soluong = 20, Trangthai = true }
            };

            _context.MatHangs.AddRange(matHangs);
            _context.SaveChanges();
        }
    }
}