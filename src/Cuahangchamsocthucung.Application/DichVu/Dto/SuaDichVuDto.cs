using System.Collections.Generic;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class SuaDichVuDto
    {
        public int Id { get; set; }
        public string Tendichvu { get; set; }
        public string Mota { get; set; }
        public int LoaiDichVu { get; set; }
        public bool Trangthai { get; set; }
        public List<SuaBangGiaDto> BangGias { get; set; } = new();
    }

    public class SuaBangGiaDto
    {
        public int Id { get; set; }
        public int DichVuId { get; set; }
        public string Loaithucung { get; set; }
        public string LoaiPhong { get; set; }
        public bool Loailong { get; set; }
        public int Cannangtu { get; set; }
        public int Cannangden { get; set; }
        public int ThoiGianPhut { get; set; }
        public decimal Giadv { get; set; }
    }
}