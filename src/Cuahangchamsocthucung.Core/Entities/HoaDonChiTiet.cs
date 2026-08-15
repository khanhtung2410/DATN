using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Entities
{
    public class HoaDonChiTiet : Entity<int>
    {
        public int HoaDonId { get; set; }

        public int DichVuId { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }

        [ForeignKey(nameof(HoaDonId))]
        public virtual HoaDon HoaDon { get; set; }

        [ForeignKey(nameof(DichVuId))]
        public virtual DichVu DichVu { get; set; }
        public HoaDonChiTiet()
        {
        }
        public HoaDonChiTiet(int hoaDonId, int dichVuId, decimal donGia, decimal thanhTien)
        {
            HoaDonId = hoaDonId;
            DichVuId = dichVuId;
            DonGia = donGia;
            ThanhTien = thanhTien;
        }
    }
}
