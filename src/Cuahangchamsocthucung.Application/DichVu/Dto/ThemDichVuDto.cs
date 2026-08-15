using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class ThemDichVuDto
    {
        [Required]
        public string Tendichvu { get; set; }

        public string Mota { get; set; }

        public bool Trangthai { get; set; }
        [MinLength(1, ErrorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc.")]
        public List<ThemBangGiaDto> BangGias { get; set; }
    }
}
