using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Entities
{
    public class HoaDon : FullAuditedEntity<int>
    {
        public int NhanVienId { get; set; }
        public int KhachHangId { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        [ForeignKey(nameof(NhanVienId))]
        public virtual NhanVien NhanVien { get; set; }
        [ForeignKey(nameof(KhachHangId))]
        public virtual KhachHang KhachHang { get; set; }
        public virtual ICollection<HoaDonChiTiet> ChiTietHoaDons { get; set; }

        public HoaDon()
        {
        }
        public HoaDon(int nhanvienId, int khachhangId, DateTime ngaylap, decimal tongtien, string trangthai)
        {
            NhanVienId = nhanvienId;
            KhachHangId = khachhangId;
            NgayLap = ngaylap;
            TongTien = tongtien;
            TrangThai = trangthai;
        }
    }
}
