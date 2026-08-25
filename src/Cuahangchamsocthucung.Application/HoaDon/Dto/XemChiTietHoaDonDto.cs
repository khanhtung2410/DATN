using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.HoaDon.Dto
{
    public class XemChiTietHoaDonDto
    {
        [Required]
        public int Id { get; set; }

        public int KhachHangId { get; set; }
        public string TenKhachHang { get; set; }
        public string SDTKhachHang { get; set; }

        public int NhanVienId { get; set; }
        public string TenNhanVien { get; set; }

        public int ThuCungId { get; set; }
        public string TenThuCung { get; set; }
        public string LoaiThuCung { get; set; }

        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }

        public int BangGiaId { get; set; }
        public string TenBangGia { get; set; }
        public string LoaiPhong { get; set; }
        public string LoaiLong { get; set; }
        public string KhoangCanNang { get; set; }
        public string LoaiThuCungBangGia { get; set; }
        public decimal DonGia { get; set; }
        public int ThoiGianPhut { get; set; }

        public DateTime NgayLap { get; set; }
        public DateTime ThoiGianTu { get; set; }
        public DateTime ThoiGianDen { get; set; }

        public decimal TongTienTruocGiam { get; set; }
        public decimal PhanTramGiam { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongTien { get; set; }

        public string TenVip { get; set; }
        public int? CapVip { get; set; }

        public string TrangThai { get; set; }

        public List<HoaDonChiTietDto> ChiTietHoaDons { get; set; }

        public XemChiTietHoaDonDto()
        {
            ChiTietHoaDons = new List<HoaDonChiTietDto>();
        }
    }
}