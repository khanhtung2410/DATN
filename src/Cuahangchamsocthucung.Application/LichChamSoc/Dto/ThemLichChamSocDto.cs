using System;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.LichChamSoc.Dto
{
    public class ThemLichChamSocDto
    {
        [Required(ErrorMessage = "Vui lòng chọn thú cưng.")]
        public int? ThuCungId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn dịch vụ.")]
        public int? DichVuId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bảng giá.")]
        public int? BangGiaId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày và giờ.")]
        public DateTime? ThoiGian { get; set; }
    }
}