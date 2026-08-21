using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultLichChamSocCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public DefaultLichChamSocCreator(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateLichChamSoc();
        }

        private void CreateLichChamSoc()
        {
            const int soLuongLichDemo = 100;

            var khachHangs = _context.KhachHangs
                .Where(x => x.TenantId == 1)
                .OrderBy(x => x.Id)
                .ToList();

            var thuCungs = _context.ThuCungs
                .Where(x => x.TrangThai)
                .OrderBy(x => x.Id)
                .ToList();

            var bangGias = _context.BangGias
                .Include(x => x.DichVu)
                .OrderBy(x => x.Id)
                .ToList();

            var nhanViens = _context.NhanViens
                .Where(x => x.Trangthai)
                .OrderBy(x => x.Id)
                .ToList();

            if (!khachHangs.Any() || !thuCungs.Any() || !bangGias.Any() || !nhanViens.Any())
                return;

            var soLichHienTai = _context.LichChamSocs.Count();

            if (soLichHienTai >= soLuongLichDemo)
                return;

            var khungGio = new[]
            {
        new TimeSpan(8, 0, 0),
        new TimeSpan(8, 30, 0),
        new TimeSpan(9, 0, 0),
        new TimeSpan(9, 30, 0),
        new TimeSpan(10, 0, 0),
        new TimeSpan(10, 30, 0),
        new TimeSpan(11, 0, 0),
        new TimeSpan(13, 30, 0),
        new TimeSpan(14, 0, 0),
        new TimeSpan(14, 30, 0),
        new TimeSpan(15, 0, 0),
        new TimeSpan(15, 30, 0),
        new TimeSpan(16, 0, 0),
        new TimeSpan(16, 30, 0),
        new TimeSpan(17, 0, 0)
    };

            var lichDaCo = _context.LichChamSocs
                .Select(x => new
                {
                    x.KhachHangId,
                    x.ThuCungId,
                    x.ThoiGian,
                    x.NhanVienId
                })
                .ToList();

            var lichMoi = new List<LichChamSoc>();

            for (int i = soLichHienTai; i < soLuongLichDemo; i++)
            {
                var khachHang = khachHangs[i % khachHangs.Count];

                var danhSachThuCung = thuCungs
                    .Where(x => x.KhachHangId == khachHang.Id)
                    .ToList();

                if (!danhSachThuCung.Any())
                {
                    i--;
                    continue;
                }

                var thuCung = danhSachThuCung[i % danhSachThuCung.Count];
                var bangGia = bangGias[i % bangGias.Count];

                DateTime thoiGian;

                // 60% lịch quá khứ, 40% lịch tương lai
                if (i % 10 < 6)
                {
                    var ngay = DateTime.Today.AddDays(-(1 + (i % 30)));
                    thoiGian = ngay.Add(khungGio[i % khungGio.Length]);
                }
                else
                {
                    var ngay = DateTime.Today.AddDays(1 + (i % 30));
                    thoiGian = ngay.Add(khungGio[i % khungGio.Length]);
                }

                var daTonTai = lichDaCo.Any(x =>
                    x.KhachHangId == khachHang.Id &&
                    x.ThuCungId == thuCung.Id &&
                    x.ThoiGian == thoiGian);

                if (daTonTai)
                {
                    i--;
                    continue;
                }

                TrangThaiLichChamSoc trangThai;

                if (thoiGian < DateTime.Now)
                {
                    switch (i % 3)
                    {
                        case 0:
                            trangThai = TrangThaiLichChamSoc.HoanThanh;
                            break;
                        case 1:
                            trangThai = TrangThaiLichChamSoc.DaHuy;
                            break;
                        default:
                            trangThai = TrangThaiLichChamSoc.BiTuChoi;
                            break;
                    }
                }
                else
                {
                    trangThai = i % 2 == 0
                        ? TrangThaiLichChamSoc.ChoXacNhan
                        : TrangThaiLichChamSoc.DaXacNhan;
                }

                int? nhanVienId = null;

                var canPhanCong =
                    trangThai == TrangThaiLichChamSoc.DaXacNhan ||
                    trangThai == TrangThaiLichChamSoc.HoanThanh;

                if (canPhanCong)
                {
                    var nhanVien = nhanViens.FirstOrDefault(nv =>
                        !lichDaCo.Any(x =>
                            x.NhanVienId == nv.Id &&
                            x.ThoiGian == thoiGian));

                    if (nhanVien == null)
                    {
                        i--;
                        continue;
                    }

                    nhanVienId = nhanVien.Id;
                }

                var lich = new LichChamSoc
                {
                    KhachHangId = khachHang.Id,
                    ThuCungId = thuCung.Id,
                    DichVuId = bangGia.DichVuId,
                    BangGiaId = bangGia.Id,
                    NhanVienId = nhanVienId,
                    ThoiGian = thoiGian,
                    TrangThai = trangThai
                };

                lichMoi.Add(lich);

                lichDaCo.Add(new
                {
                    KhachHangId = khachHang.Id,
                    ThuCungId = thuCung.Id,
                    ThoiGian = thoiGian,
                    NhanVienId = nhanVienId
                });
            }

            if (lichMoi.Any())
            {
                _context.LichChamSocs.AddRange(lichMoi);
                _context.SaveChanges();
            }
        }
    }
}