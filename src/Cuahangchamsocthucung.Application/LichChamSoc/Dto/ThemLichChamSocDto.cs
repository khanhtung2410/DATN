using System;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.LichChamSoc.Dto
{
    public class ThemLichChamSocDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn thú cưng.")]
        public int ThuCungId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn dịch vụ.")]
        public int DichVuId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn bảng giá.")]
        public int BangGiaId { get; set; }
        public decimal CanNang { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày và giờ.")]
        public DateTime ThoiGian { get; set; }
    }
}