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
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        public string HoTen { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SDT { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.#])[A-Za-z\d@$!%*?&.#]{8,}$",
    ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public string MatKhau { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
        [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string XacNhanMatKhau { get; set; }
    }
    public class XacThucOtpDto
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Mã OTP không được để trống.")]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "Mã OTP phải gồm 6 chữ số.")]
        public string Otp { get; set; }
    }
}
