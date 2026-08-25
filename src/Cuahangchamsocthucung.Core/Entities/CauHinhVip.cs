using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchamsocthucung.Entities
{
    public class CauHinhVip : Entity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int VipId { get; set; }

        [ForeignKey(nameof(VipId))]
        public virtual Vip Vip { get; set; }

        [Range(0, 100)]
        public decimal PhanTramGiam { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MucChiTieu { get; set; }

        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
}