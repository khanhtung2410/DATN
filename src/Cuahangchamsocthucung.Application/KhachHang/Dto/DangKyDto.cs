using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.KhachHang.Dto
{
    public class DangKyDto
    {
        [Required]
        public string HoTen { get; set; }
        [Required]
        [Phone]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SDT { get; set; }
        public string? Email { get; set; }
        [Required]
        [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.#])[A-Za-z\d@$!%*?&.#]{8,}$",
    ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public string MatKhau { get; set; }
        [Required]
        [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string XacNhanMatKhau { get; set; }
    }
    public class XacThucOtpDto
    {
        [Required]
        [Phone]
        public string SDT { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; }
    }
}
