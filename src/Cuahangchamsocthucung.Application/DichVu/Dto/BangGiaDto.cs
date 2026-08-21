using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class BangGiaDto
    {
        [Required] 
        public int Id { get; set; }
        [Required] 
        public int DichvuId { get; set; }
        [Required]
        public string Loaithucung { get; set; }
        public string LoaiPhong { get; set; }
        public bool Loailong { get; set; }
        public int Cannangtu { get; set; }
        public int Cannangden { get; set; }
        public decimal Giadv { get; set; }
        [Required, Range(1, 1440, ErrorMessage = "Thời gian phải từ 1 đến 1440 phút.")] public int ThoiGianPhut { get; set; }
    }
}