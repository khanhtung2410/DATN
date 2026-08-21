using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchamsocthucung.Entities
{
    public class HoaDon : FullAuditedEntity<int>
    {
        public int LichChamSocId { get; set; }
        public int NhanVienId { get; set; }
        public int KhachHangId { get; set; }
        public DateTime NgayLap { get; set; }

        // Giá trước khi giảm
        public decimal TongTienTruocGiam { get; set; }

        // Phần trăm giảm theo cấp VIP
        public decimal PhanTramGiam { get; set; }

        // Số tiền được giảm
        public decimal TienGiam { get; set; }

        // Số tiền khách phải thanh toán
        public decimal TongTien { get; set; }

        public string TrangThai { get; set; }

        [ForeignKey(nameof(LichChamSocId))]
        public virtual LichChamSoc LichChamSoc { get; set; }

        [ForeignKey(nameof(NhanVienId))]
        public virtual NhanVien NhanVien { get; set; }

        [ForeignKey(nameof(KhachHangId))]
        public virtual KhachHang KhachHang { get; set; }

        public virtual ICollection<HoaDonChiTiet> ChiTietHoaDons { get; set; }

        public HoaDon()
        {
            ChiTietHoaDons = new List<HoaDonChiTiet>();
        }

        public HoaDon(
            int lichChamSocId,
            int nhanVienId,
            int khachHangId,
            DateTime ngayLap,
            decimal tongTienTruocGiam,
            decimal phanTramGiam,
            decimal tienGiam,
            decimal tongTien,
            string trangThai)
        {
            LichChamSocId = lichChamSocId;
            NhanVienId = nhanVienId;
            KhachHangId = khachHangId;
            NgayLap = ngayLap;
            TongTienTruocGiam = tongTienTruocGiam;
            PhanTramGiam = phanTramGiam;
            TienGiam = tienGiam;
            TongTien = tongTien;
            TrangThai = trangThai;
            ChiTietHoaDons = new List<HoaDonChiTiet>();
        }
    }
}