using Cuahangchamsocthucung.Enum;
using System;
namespace Cuahangchamsocthucung.LichChamSoc.Dto {
    public class LichChamSocDto
    {
        public int Id { get; set; }
        public int ThuCungId { get; set; }
        public string TenThuCung { get; set; }
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public int BangGiaId { get; set; }
        public int? NhanVienId { get; set; }
        public string TenNhanVien { get; set; }
        public int KhachHangId { get; set; }
        public string TenKhachHang { get; set; }
        public DateTime ThoiGian { get; set; }
        public TrangThaiLichChamSoc TrangThai { get; set; }
    }
    public class LichChamSocTimelineDto
    {
        public int Id { get; set; }

        public int? NhanVienId { get; set; }
        public string TenNhanVien { get; set; }

        public string TenThuCung { get; set; }
        public string TenDichVu { get; set; }
        public string TenKhachHang { get; set; }

        public DateTime ThoiGian { get; set; }

        public int ThoiGianPhut { get; set; }

        public DateTime ThoiGianKetThuc =>
            ThoiGian.AddMinutes(ThoiGianPhut);

        public TrangThaiLichChamSoc TrangThai { get; set; }
    }
    public class LichChamSocChuaCoHoaDonDto
    {
        public int Id { get; set; }
        public int KhachHangId { get; set; }
        public string TenKhachHang { get; set; }
        public string SDTKhachHang { get; set; }
        public int? NhanVienId { get; set; }
        public string TenNhanVien { get; set; }
        public int ThuCungId { get; set; }
        public string TenThuCung { get; set; }
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public DateTime ThoiGian { get; set; }
        public decimal DonGia { get; set; }
        public int ThoiGianPhut { get; set; }
    }
}
