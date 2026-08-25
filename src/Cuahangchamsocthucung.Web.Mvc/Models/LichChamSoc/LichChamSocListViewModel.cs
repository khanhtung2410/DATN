using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.LichChamSoc.Dto;
using System;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.Web.Models.LichChamSoc
{
    public class LichChamSocListViewModel
    {
        public List<LichChamSocDto> LichChamSocs { get; set; } = new List<LichChamSocDto>();
        public string TenKhachHang { get; set; }
        public TrangThaiLichChamSoc? TrangThai { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
    public class LichChamSocTimelineViewModel
    {
        public DateTime Ngay { get; set; }

        public int? NhanVienId { get; set; }

        public List<LichChamSocTimelineDto> LichChamSocs { get; set; }
            = new List<LichChamSocTimelineDto>();
    }
}
