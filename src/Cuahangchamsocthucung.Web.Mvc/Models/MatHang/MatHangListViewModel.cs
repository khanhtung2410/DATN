using Cuahangchamsocthucung.MatHang.Dto;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.Web.Models.MatHang
{
    public class MatHangListViewModel
    {
        public IReadOnlyList<MatHangDto> MatHangs { get; set; }
    }
}
