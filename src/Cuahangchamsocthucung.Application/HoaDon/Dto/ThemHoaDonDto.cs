using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.HoaDon.Dto
{
    public class ThemHoaDonDto
    {
        public int KhachHangId { get; set; }
        public int NhanVienId { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public List<ThemHoaDonChiTietDto> ChiTietHoaDon { get; set; }
    }
    public class ThemHoaDonChiTietDto
    {
        [Required]
        public int HoaDonId { get; set; }
        public int DichVuId { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
