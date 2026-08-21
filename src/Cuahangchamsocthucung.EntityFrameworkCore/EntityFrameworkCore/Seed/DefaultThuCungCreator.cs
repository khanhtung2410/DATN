using System.Collections.Generic;
using System.Linq;
using Cuahangchamsocthucung.Entities;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultThuCungCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultThuCungCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateThuCung();
        }

        private void CreateThuCung()
        {
            var khachHangs = _context.KhachHangs
                .Where(x => x.TenantId == 1)
                .OrderBy(x => x.Id)
                .ToList();

            var pets = new Dictionary<string, List<(string Ten, string Loai, string GhiChu, string Image)>>
            {
                ["0912345678"] = new()
                {
                    ("Lucky", "Chó", "Chó Shiba", "/img/thu-cung-duoi-10kg.jpg"),
                    ("Milo", "Chó", "Chó Poodle", "/img/poodle-1")
                },
                ["0912345679"] = new()
                {
                    ("Nâu", "Chó", "Chó Poodle", "/img/poodle-2.jpg")
                },
                ["0912345680"] = new()
                {
                    ("Max", "Chó", "Chó Golden Retriever", "/img/golden-3.jpg"),
                    ("Tom", "Mèo", "Mèo Anh lông dài", "/img/meo-anh-long-dai-1.jpg"),
                    ("Mun", "Mèo", "Mèo ta", "/img/meo-ta-1.jpg")
                },
                ["0912345681"] = new()
                {
                    ("Cún", "Chó", "Chó Chihuahua", "/img/chihuahua-1.jpg"),
                    ("Bibi", "Mèo", "Mèo Scottish Fold", "/img/Scottish Fold-1.jpg")
                },
                ["0912345682"] = new()
                {
                    ("Rocky", "Chó", "Chó Husky", "/img/thu-cung-duoi-20kg.jpg"),
                    ("Bông Tuyết", "Chó", "Chó Samoyed", "/img/samoyed-1.jpg"),
                    ("Miu", "Mèo", "Mèo Ba Tư", "/img/ba-tu-01.jpg")
                },
                ["0912345683"] = new()
                {
                    ("Na", "Chó", "Chó Bắc Kinh", "/img/cho-bac-kinh-1.jpg"),
                    ("Tí", "Mèo", "Mèo ta", "/img/meo-ta-2.jpg")
                },
                ["0912345684"] = new()
                {
                    ("Miu Miu", "Mèo", "Mèo Anh lông ngắn", "/img/meo-anh-long-ngan-1.jpg")
                },
                ["0912345685"] = new()
                {
                    ("Kem", "Chó", "Chó Poodle", "/img/poodle-3.jpg"),
                    ("Đậu", "Mèo", "Mèo ta", "/img/meo-ta-3.jpg")
                },
                ["0912345686"] = new()
                {
                    ("Simba", "Chó", "Chó Golden Retriever", "/img/golden-1.jpg"),
                    ("Leo", "Mèo", "Mèo Bengal", "/img/bengal-1.jpg"),
                    ("Mochi", "Chó", "Chó Corgi", "/img/corgi-01.jpg")
                },
                ["0912345687"] = new()
                {
                    ("Mây", "Chó", "Chó Poodle", "/img/poodle-4.jpg"),
                    ("Sữa", "Mèo", "Mèo Anh lông ngắn", "/img/meo-anh-long-ngan-2.jpg")
                },
                ["0912345688"] = new()
                {
                    ("Nana", "Mèo", "Mèo Ba Tư", "/img/thu-cung-duoi-10kg.jpg")
                },
                ["0912345689"] = new()
                {
                    ("Milo", "Chó", "Chó Corgi", "/img/corgi-02.jpg"),
                    ("Miu", "Mèo", "Mèo Scottish Fold", "/img/Scottish Fold-2.jpg")
                },
                ["0912345690"] = new()
                {
                    ("Tôm", "Mèo", "Mèo ta", "/img/ba-tu-02.jpg"),
                    ("Cún Con", "Chó", "Chó Chihuahua", "/img/chihuahua-2.jpg")
                },
                ["0912345691"] = new()
                {
                    ("Bơ", "Chó", "Chó Poodle", "/img/poodle-5.jpg"),
                    ("Mun", "Mèo", "Mèo Anh lông ngắn", "/img/meo-anh-long-ngan-3.jpg")
                },
                ["0912345692"] = new()
                {
                    ("Oscar", "Chó", "Chó Golden Retriever", "/img/golden-2.jpg"),
                    ("Đốm", "Chó", "Chó Dalmatian", "/img/dalmation-01.jpg")
                }
            };

            foreach (var khachHang in khachHangs)
            {
                if (!pets.TryGetValue(khachHang.SDT, out var danhSach))
                    continue;

                foreach (var pet in danhSach)
                {
                    if (_context.ThuCungs.Any(x => x.KhachHangId == khachHang.Id && x.TenThuCung == pet.Ten))
                        continue;

                    _context.ThuCungs.Add(new ThuCung
                    {
                        KhachHangId = khachHang.Id,
                        TenThuCung = pet.Ten,
                        LoaiThuCung = pet.Loai,
                        GhiChu = pet.GhiChu,
                        TrangThai = true,
                        ImageUrl = pet.Image
                    });
                }
            }

            _context.SaveChanges();
        }
    }
}