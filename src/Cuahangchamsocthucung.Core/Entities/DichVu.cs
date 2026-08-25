using Abp.Domain.Entities.Auditing;
using Cuahangchamsocthucung.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.Entities
{
    public class DichVu : AuditedEntity<int>
    {
        [Required]
        public string TenDichVu { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public LoaiDichVu LoaiDichVu { get; set; }
        public virtual ICollection<BangGia> BangGias { get; set; }

        public DichVu(string tendichvu, string mota, bool trangthai, LoaiDichVu loaiDichVu)
        {
            TenDichVu = tendichvu;
            MoTa = mota;
            TrangThai = trangthai;
            LoaiDichVu = loaiDichVu;
            BangGias = new List<BangGia>();
        }

        public DichVu()
        {
            BangGias = new List<BangGia>();
        }
    }
}