using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class ThemBangGiaDto
    {
        [Required(ErrorMessage = "Vui lòng chọn đối tượng.")]
        public string Loaithucung { get; set; }
        public string LoaiPhong { get; set; }
        public bool Loailong { get; set; }
        [Range(0, int.MaxValue)]
        public int Cannangtu { get; set; }
        [Range(1, int.MaxValue)]
        public int Cannangden { get; set; }
        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Giadv { get; set; }
        [Range(1, 1440, ErrorMessage = "Thời gian phải từ 1 đến 1440 phút.")]
        public int ThoiGianPhut { get; set; }
    }
}