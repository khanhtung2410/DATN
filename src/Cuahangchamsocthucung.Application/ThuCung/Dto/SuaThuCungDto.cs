using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.ThuCung.Dto
{
    public class SuaThuCungDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thú cưng")]
        [StringLength(100)]
        public string TenThuCung { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại thú cưng")]
        [StringLength(50)]
        public string LoaiThuCung { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public bool TrangThai { get; set; }

        [StringLength(400)]
        public string ImageUrl { get; set; }
    }
}
