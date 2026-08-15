using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class ThemBangGiaDto
    {
        [Required]
        public int DichVuId { get; set; }
        [Required]
        public string Loaithucung { get; set; }
        public bool Loailong { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Cannangtu { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Cannangden { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Giadv { get; set; }
    }
}
