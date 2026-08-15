using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.HoaDon.Dto
{
    public class HoaDonDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int KhachHangId { get; set; }
        [Required]
        public int NhanVienId { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }

    }
    public class HoaDonChiTietDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int HoaDonId { get; set; }
        [Required]
        public int DichVuId { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
