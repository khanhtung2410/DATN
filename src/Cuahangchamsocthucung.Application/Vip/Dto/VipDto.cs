using System;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.Vip.Dto
{
    public class VipDto
    {
        public int Id { get; set; }
        public int CapVip { get; set; }
        public string TenVip { get; set; }
    }

    public class ThemVipDto
    {
        [Required(ErrorMessage = "Tên VIP không được để trống.")]
        public string TenVip { get; set; }

        [Range(1, 5, ErrorMessage = "Cấp VIP phải từ 1 đến 5.")]
        public int CapVip { get; set; }
    }

    public class SuaVipDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên VIP không được để trống.")]
        public string TenVip { get; set; }

        [Range(1, 5, ErrorMessage = "Cấp VIP phải từ 1 đến 5.")]
        public int CapVip { get; set; }
    }

    public class CauHinhVipDto
    {
        public int Id { get; set; }
        public int VipId { get; set; }
        public decimal PhanTramGiam { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class ThemCauHinhVipDto
    {
        public int VipId { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm giảm phải từ 0 đến 100.")]
        public decimal PhanTramGiam { get; set; }

        public DateTime TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }
    }

    public class SuaCauHinhVipDto
    {
        public int Id { get; set; }
        public int VipId { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm giảm phải từ 0 đến 100.")]
        public decimal PhanTramGiam { get; set; }

        public DateTime TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }
    }
}