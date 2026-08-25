using Cuahangchamsocthucung.HoaDon.Dto;
using System;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.Web.Models.HoaDon
{
    public class HoaDonListViewModel
    {
        public List<HoaDonDto> HoaDons { get; set; } = new List<HoaDonDto>();
        public string TenKhachHang { get; set; }
        public string TrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}