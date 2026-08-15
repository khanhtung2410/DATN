using Abp.Domain.Entities;
using Cuahangchamsocthucung.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Entities
{
    public class KhachHang : Entity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        [Required]
        public string Hoten { get; set; }
        [Required]
        [Phone]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SDT { get; set; }
        public string? Email { get; set; }
        public KhachHang()
        {
        }
        public KhachHang(string hoten, string sdt, string email)
        {
            Hoten = hoten;
            SDT = sdt;
            Email = email;
        }
    }
}
