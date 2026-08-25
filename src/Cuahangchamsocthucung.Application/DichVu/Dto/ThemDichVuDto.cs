using Cuahangchamsocthucung.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class ThemDichVuDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ.")]
        public string Tendichvu { get; set; }
        public string Mota { get; set; }
        public bool Trangthai { get; set; } = true;
        [Required]
        public LoaiDichVu LoaiDichVu { get; set; }
        [MinLength(1, ErrorMessage = "Vui lòng nhập ít nhất một bảng giá.")]
        public List<ThemBangGiaDto> BangGias { get; set; } = new List<ThemBangGiaDto>();
    }
}