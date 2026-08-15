using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.MatHang.Dto
{
    public class SuamathangDto
    {
        public int Id { get; set; }
        public string Tenmathang { get; set; }
        public string Mota { get; set; }
        public int Soluong { get; set; }
        public bool Trangthai { get; set; }
    }
    public class SuaTrangThaiMatHangDto
    {
        public int Id { get; set; }
        public bool Trangthai { get; set; }
    }
}
