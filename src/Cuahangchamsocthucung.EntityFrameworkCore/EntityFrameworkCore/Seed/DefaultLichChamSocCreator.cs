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
            // 360 lịch trong 3 tháng
            const int soLuongLichDemo = 360;

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
                .Where(x => x.DichVu != null)
                .OrderBy(x => x.Id)
                .ToList();

            var nhanViens = _context.NhanViens
                .Where(x => x.Trangthai)
                .OrderBy(x => x.Id)
                .ToList();

            if (!khachHangs.Any() ||
                !thuCungs.Any() ||
                !bangGias.Any() ||
                !nhanViens.Any())
            {
                return;
            }

            var soLichHienTai = _context.LichChamSocs.Count();

            if (soLichHienTai >= soLuongLichDemo)
                return;

            var khachHangCoThuCung = khachHangs
                .Where(kh => thuCungs.Any(tc => tc.KhachHangId == kh.Id))
                .ToList();

            if (!khachHangCoThuCung.Any())
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

            var random = new Random(20260824);

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

            var soLichCanThem = soLuongLichDemo - soLichHienTai;

            var ngayBatDau = DateTime.Today.AddMonths(-3);
            var ngayKetThuc = DateTime.Today.AddDays(-1);

            var danhSachNgay = new List<DateTime>();

            // Tạo tất cả các ngày trong 3 tháng
            for (var ngay = ngayBatDau.Date;
                 ngay <= ngayKetThuc.Date;
                 ngay = ngay.AddDays(1))
            {
                // Mỗi ngày tạo 3-5 lịch
                var soLichTrongNgay = random.Next(3, 6);

                for (var i = 0; i < soLichTrongNgay; i++)
                {
                    danhSachNgay.Add(ngay);
                }
            }

            danhSachNgay = danhSachNgay
                .OrderBy(x => random.Next())
                .ToList();

            int daTao = 0;

            foreach (var ngay in danhSachNgay)
            {
                if (daTao >= soLichCanThem)
                    break;

                for (int thuTu = 0; thuTu < khungGio.Length; thuTu++)
                {
                    if (daTao >= soLichCanThem)
                        break;

                    var khachHang =
                        khachHangCoThuCung[
                            daTao % khachHangCoThuCung.Count
                        ];

                    var danhSachThuCung = thuCungs
                        .Where(x => x.KhachHangId == khachHang.Id)
                        .ToList();

                    if (!danhSachThuCung.Any())
                        continue;

                    var thuCung =
                        danhSachThuCung[
                            daTao % danhSachThuCung.Count
                        ];

                    var bangGia =
                        bangGias[
                            daTao % bangGias.Count
                        ];

                    var thoiGian =
                        ngay.Add(khungGio[thuTu]);

                    // Không cho cùng thú cưng trùng đúng thời gian
                    var thuCungBiTrung = lichDaCo.Any(x =>
                        x.ThuCungId == thuCung.Id &&
                        x.ThoiGian == thoiGian);

                    if (thuCungBiTrung)
                        continue;

                    /*
                     * Tỷ lệ trạng thái:
                     * 80% Hoàn thành
                     * 10% Đã hủy
                     * 5% Bị từ chối
                     * 5% Đã xác nhận
                     */
                    var randomTrangThai = random.Next(100);

                    TrangThaiLichChamSoc trangThai;

                    if (randomTrangThai < 80)
                    {
                        trangThai =
                            TrangThaiLichChamSoc.HoanThanh;
                    }
                    else if (randomTrangThai < 90)
                    {
                        trangThai =
                            TrangThaiLichChamSoc.DaHuy;
                    }
                    else if (randomTrangThai < 95)
                    {
                        trangThai =
                            TrangThaiLichChamSoc.BiTuChoi;
                    }
                    else
                    {
                        trangThai =
                            TrangThaiLichChamSoc.DaXacNhan;
                    }

                    int? nhanVienId = null;

                    // Chỉ phân công nhân viên cho lịch hoàn thành
                    // hoặc đã xác nhận
                    if (trangThai ==
                            TrangThaiLichChamSoc.HoanThanh ||
                        trangThai ==
                            TrangThaiLichChamSoc.DaXacNhan)
                    {
                        var nhanVien =
                            nhanViens
                                .Where(nv =>
                                    !lichDaCo.Any(x =>
                                        x.NhanVienId == nv.Id &&
                                        x.ThoiGian == thoiGian))
                                .OrderBy(x => random.Next())
                                .FirstOrDefault();

                        if (nhanVien == null)
                            continue;

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

                    daTao++;
                }
            }

            if (lichMoi.Any())
            {
                _context.LichChamSocs.AddRange(lichMoi);
                _context.SaveChanges();
            }
        }
    }
}