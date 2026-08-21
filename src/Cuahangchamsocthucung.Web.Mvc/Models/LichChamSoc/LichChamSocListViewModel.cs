using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.LichChamSoc.Dto;
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
    }
}
