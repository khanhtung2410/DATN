using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Cuahangchamsocthucung.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Entities
{
    public class LichChamSoc : FullAuditedEntity<int>
    {
        public int DichVuId { get; set; }
        public int BangGiaId { get; set; }
        public int? NhanVienId { get; set; }
        public int KhachHangId { get; set; }
        public DateTime ThoiGian { get; set; }
        public TrangThaiLichChamSoc TrangThai { get; set; }
        // ChoXacNhan, DaXacNhan, DangDienRa, HoanThanh, DaHuy
        [ForeignKey(nameof(BangGiaId))]
        public virtual BangGia BangGia { get; set; }
        [ForeignKey(nameof(KhachHangId))]
        public virtual KhachHang KhachHang { get; set; }
        [ForeignKey(nameof(DichVuId))]
        public virtual DichVu DichVu { get; set; }

        [ForeignKey(nameof(NhanVienId))]
        public virtual NhanVien NhanVien { get; set; }
        public LichChamSoc(int dichVuId, int? nhanVienId, int khachHangId, int BanggiaId, DateTime thoiGian)
        {
            KhachHangId = khachHangId;
            BangGiaId = BanggiaId;
            DichVuId = dichVuId;
            NhanVienId = nhanVienId;
            ThoiGian = thoiGian;
            TrangThai = TrangThaiLichChamSoc.ChoXacNhan;
        }
        public LichChamSoc()
        {
        }
    }
}
