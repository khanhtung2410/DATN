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
    public class MatHang: AuditedEntity<int>
    {
        [Required]
        public string Tenmathang { get; set; }
        public string Mota { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ.")]
        public int Soluong { get; set; }
        public bool Trangthai { get; set; }
        public MatHang()
        {
        }
        public MatHang(string tenmathang, string mota, bool trangthai)
        {
            Tenmathang = tenmathang;
            Mota = mota;
            Trangthai = trangthai;
        }
    }
}
