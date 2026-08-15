using Cuahangchamsocthucung.KhachHang.Dto;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.Web.Models.KhachHang
{
    public class KhachHangListViewModel
    {
        public IReadOnlyList<KhachHangDto> KhachHangs { get; set; }
    }
}
