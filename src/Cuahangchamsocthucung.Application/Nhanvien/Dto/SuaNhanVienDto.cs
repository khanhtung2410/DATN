using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Nhanvien.Dto
{
    public class SuaNhanVienDto
    {
        public int Id { get; set; }
        public string Hoten { get; set; }
        public string SDT { get; set; }
        public bool Gioitinh { get; set; }
        public DateOnly Ngaysinh { get; set; }
        public DateOnly Ngayvaolam { get; set; }
        public bool Trangthai { get; set; }
    }
    public class SuaTrangThaiNhanVienDto
    {
        public int Id { get; set; }
        public bool Trangthai { get; set; }
    }
}
