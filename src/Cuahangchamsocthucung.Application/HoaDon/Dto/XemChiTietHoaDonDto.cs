using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.HoaDon.Dto
{
    public class XemChiTietHoaDonDto
    {
        [Required]
        public int Id { get; set; }
        public int KhachHangId { get; set; }
        public int NhanVienId { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public List<HoaDonChiTietDto> ChiTietHoaDons { get; set; }
    }
}
