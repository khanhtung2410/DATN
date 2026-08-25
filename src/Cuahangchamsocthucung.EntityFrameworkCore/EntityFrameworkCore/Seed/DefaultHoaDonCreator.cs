using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultHoaDonCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultHoaDonCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHoaDons();
        }

        private void CreateHoaDons()
        {
            var lichs = _context.LichChamSocs
                .Include(x => x.BangGia)
                .Include(x => x.KhachHang)
                    .ThenInclude(x => x.Vip)
                        .ThenInclude(x => x.CauHinhVips)
                .Where(x => x.TrangThai == TrangThaiLichChamSoc.HoanThanh)
                .OrderBy(x => x.Id)
                .ToList();

            var nhanViens = _context.NhanViens
                .Where(x => x.Trangthai)
                .OrderBy(x => x.Id)
                .ToList();

            if (!lichs.Any() || !nhanViens.Any())
                return;

            foreach (var lich in lichs)
            {
                if (_context.HoaDons.Any(x => x.LichChamSocId == lich.Id && !x.IsDeleted))
                    continue;

                var donGia = lich.BangGia?.Giadv ?? 0;

                if (donGia <= 0)
                    continue;

                var phanTramGiam = 0m;

                var cauHinh = lich.KhachHang?.Vip?.CauHinhVips?
                    .Where(x => x.TuNgay <= lich.ThoiGian &&
                               (!x.DenNgay.HasValue || x.DenNgay.Value >= lich.ThoiGian))
                    .OrderByDescending(x => x.TuNgay)
                    .FirstOrDefault();

                if (cauHinh != null)
                    phanTramGiam = cauHinh.PhanTramGiam;

                var tienGiam = donGia * phanTramGiam / 100;
                var tongTien = donGia - tienGiam;
                var nhanVienId = lich.NhanVienId ?? nhanViens.First().Id;

                var hoaDon = new HoaDon(
                    lich.Id,
                    nhanVienId,
                    lich.KhachHangId,
                    lich.ThoiGian,
                    donGia,
                    phanTramGiam,
                    tienGiam,
                    tongTien,
                    "DaThanhToan"
                );

                _context.HoaDons.Add(hoaDon);
                _context.SaveChanges();

                _context.HoaDonChiTiets.Add(new HoaDonChiTiet(
                    hoaDon.Id,
                    lich.DichVuId,
                    donGia,
                    tongTien
                ));

                _context.SaveChanges();
            }

            CapNhatVip();
        }

        private void CapNhatVip()
        {
            var khachHangs = _context.KhachHangs
                .Where(x => x.TenantId == 1)
                .ToList();

            var vips = _context.Vips
                .Where(x => x.TenantId == 1)
                .OrderByDescending(x => x.CapVip)
                .ToList();

            foreach (var khachHang in khachHangs)
            {
                var tongTien = _context.HoaDons
                    .Where(x => x.KhachHangId == khachHang.Id &&
                                x.TrangThai == "DaThanhToan" &&
                                !x.IsDeleted)
                    .Sum(x => (decimal?)x.TongTien) ?? 0;

                var capVip = tongTien >= 20000000 ? 5 :
                             tongTien >= 10000000 ? 4 :
                             tongTien >= 5000000 ? 3 :
                             tongTien >= 3000000 ? 2 :
                             tongTien >= 1000000 ? 1 : 0;

                var vip = vips.FirstOrDefault(x => x.CapVip == capVip);
                khachHang.VipId = vip?.Id;
            }

            _context.SaveChanges();
        }
    }
}