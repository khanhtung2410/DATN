using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.ThuCung.Dto
{
    public class ThuCungDto
    {
        public int Id { get; set; }
        [Required]
        public int KhachHangId { get; set; }

        public string TenThuCung { get; set; }

        public string LoaiThuCung { get; set; }

        public string GhiChu { get; set; }

        public bool TrangThai { get; set; }

        public string ImageUrl { get; set; }
    }
}
