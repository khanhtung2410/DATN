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
        public int ThuCungId { get; set; }
        public int DichVuId { get; set; }
        public int BangGiaId { get; set; }
        public int? NhanVienId { get; set; }
        public int KhachHangId { get; set; }
        public DateTime ThoiGian { get; set; }
        public TrangThaiLichChamSoc TrangThai { get; set; }

        // =========================
        // FOREIGN KEY
        // =========================

        [ForeignKey(nameof(ThuCungId))]
        public virtual ThuCung ThuCung { get; set; }

        [ForeignKey(nameof(BangGiaId))]
        public virtual BangGia BangGia { get; set; }

        [ForeignKey(nameof(KhachHangId))]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey(nameof(DichVuId))]
        public virtual DichVu DichVu { get; set; }

        [ForeignKey(nameof(NhanVienId))]
        public virtual NhanVien NhanVien { get; set; }


        public LichChamSoc()
        {
        }

        public LichChamSoc(
            int thuCungId,
            int dichVuId,
            int bangGiaId,
            int? nhanVienId,
            int khachHangId,
            DateTime thoiGian)
        {
            ThuCungId = thuCungId;
            DichVuId = dichVuId;
            BangGiaId = bangGiaId;
            NhanVienId = nhanVienId;
            KhachHangId = khachHangId;
            ThoiGian = thoiGian;

            TrangThai = TrangThaiLichChamSoc.ChoXacNhan;
        }
    }
}
