using Cuahangchamsocthucung.ThuCung.Dto;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.Web.Models.KhachHang
{
    public class KhachHangChiTietViewModel
    {
        public int Id { get; set; }
        public string Hoten { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public bool TrangThai { get; set; }
        public string TenVip { get; set; }
        public int CapVip { get; set; }
        public decimal TongChiTieu { get; set; }
        public string TenVipTiepTheo { get; set; }
        public decimal MucChiTieuVipTiepTheo { get; set; }
        public decimal ConThieuVip { get; set; }
        public List<ThuCungDto> ThuCungs { get; set; }
    }
}