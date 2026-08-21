using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class ThemBangGiaDto
    {
        [Required]
        public int DichVuId { get; set; }
        [Required]
        public string Loaithucung { get; set; }
        public string LoaiPhong { get; set; }
        public bool Loailong { get; set; }
        [Required, Range(0, int.MaxValue)] 
        public int Cannangtu { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Cannangden { get; set; }
        [Required, Range(typeof(decimal), "0.01", "999999999")] 
        public decimal Giadv { get; set; }
        [Required, Range(1, 1440, ErrorMessage = "Thời gian phải từ 1 đến 1440 phút.")] 
        public int ThoiGianPhut { get; set; }
    }
}