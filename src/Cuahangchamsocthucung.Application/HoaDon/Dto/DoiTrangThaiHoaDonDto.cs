using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.HoaDon.Dto
{
    public class DoiTrangThaiHoaDonDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string TrangThai { get; set; }
    }
}
