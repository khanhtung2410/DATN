using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.NhanVien.Dto
{
    public class ThemNhanVienDto
    {
        public string Hoten { get; set; }
        public string SDT { get; set; }
        public bool Gioitinh { get; set; }
        public DateOnly? Ngaysinh { get; set; }
        public DateOnly Ngayvaolam { get; set; }
        public decimal Luong { get; set; }
        public bool Trangthai { get; set; }
    }
}
