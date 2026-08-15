using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Entities
{
    public class DichVu: AuditedEntity<int>
    {
        [Required]
        public string TenDichVu { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public virtual ICollection<BangGia> BangGias { get; set; }
        public DichVu(string tendichvu, string mota, bool trangthai)
        {
            TenDichVu = tendichvu;
            MoTa = mota;
            TrangThai = trangthai;
        }
        public DichVu()
        {
        }

    }

}
