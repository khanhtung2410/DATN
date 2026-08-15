using Cuahangchamsocthucung.NhanVien.Dto;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.Web.Models.NhanVien
{
    public class NhanVienListViewModel
    {
        public IReadOnlyList<NhanVienDto> NhanViens { get; set; }
    }
}
