using Cuahangchamsocthucung.Enum;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class DichVuDto
    {
        public int Id { get; set; }
        public string Tendichvu { get; set; }
        public string Mota { get; set; }
        public bool Trangthai { get; set; }
        public LoaiDichVu LoaiDichVu { get; set; }
        public List<BangGiaDto> BangGias { get; set; }
    }
}