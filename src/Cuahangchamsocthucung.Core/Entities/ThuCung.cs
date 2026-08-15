using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Entities
{
    public class ThuCung : FullAuditedEntity<int>
    {
        public int KhachHangId { get; set; }
        public string TenThuCung { get; set; }
        public string LoaiThuCung { get; set; }
        public string GhiChu { get; set; }
        public bool TrangThai { get; set; }
        [StringLength(400)]
        public string ImageUrl { get; set; }
        public virtual KhachHang KhachHang { get; set; }
        public ThuCung() { }
        public ThuCung(int khachHangId, string tenThuCung, string loaiThuCung, string ghiChu, bool trangThai, string imageUrl)
        {
            KhachHangId = khachHangId;
            TenThuCung = tenThuCung;
            LoaiThuCung = loaiThuCung;
            GhiChu = ghiChu;
            TrangThai = trangThai;
            ImageUrl = imageUrl;
        }
    }
}
