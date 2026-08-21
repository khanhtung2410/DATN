using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.Entities
{
    public class Vip : Entity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(100)]
        public string TenVip { get; set; }

        public int CapVip { get; set; }

        public virtual ICollection<CauHinhVip> CauHinhVips { get; set; }

        public virtual ICollection<KhachHang> KhachHangs { get; set; }

        public Vip()
        {
            CauHinhVips = new List<CauHinhVip>();
            KhachHangs = new List<KhachHang>();
        }
    }
}