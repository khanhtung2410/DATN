using System;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.HoaDon.Dto
{
    public class HoaDonDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int KhachHangId { get; set; }
        public string TenKhachHang { get; set; }
        [Required]
        public int NhanVienId { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTienTruocGiam { get; set; }
        public decimal PhanTramGiam { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongTien { get; set; }
        public string TenVip { get; set; }
        public int? CapVip { get; set; }
        public string TrangThai { get; set; }
    }

    public class HoaDonChiTietDto
    {
        [Required]
        public int Id { get; set; }
        public int HoaDonId { get; set; }
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}