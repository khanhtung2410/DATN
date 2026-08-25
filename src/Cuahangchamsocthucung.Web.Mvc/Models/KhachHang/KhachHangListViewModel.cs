using Cuahangchamsocthucung.KhachHang.Dto;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.Web.Models.KhachHang
{
    public class KhachHangListViewModel
    {
        public List<KhachHangDto> KhachHangs { get; set; } = new List<KhachHangDto>();
    }
}