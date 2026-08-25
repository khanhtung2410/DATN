using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using System.Collections.Generic;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultDichVuCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultDichVuCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDichVu();
        }

        private void CreateDichVu()
        {
            if (_context.DichVus.Any())
                return;

            var dichVus = new List<DichVu>
            {
                new DichVu
                {
                    TenDichVu = "Trông giữ thú cưng",
                    MoTa = "Dịch vụ khách sạn lưu trú cho chó mèo",
                    TrangThai = true,
                    LoaiDichVu = LoaiDichVu.LuuTru
                },
                new DichVu
                {
                    TenDichVu = "Tắm Spa",
                    MoTa = "Tắm vệ sinh, chăm sóc lông",
                    TrangThai = true,
                    LoaiDichVu = LoaiDichVu.ChamSoc
                },
                new DichVu
                {
                    TenDichVu = "Cắt tỉa lông",
                    MoTa = "Cắt tỉa tạo kiểu cho thú cưng",
                    TrangThai = true,
                    LoaiDichVu = LoaiDichVu.ChamSoc
                }
            };

            _context.DichVus.AddRange(dichVus);
            _context.SaveChanges();
        }
    }
}