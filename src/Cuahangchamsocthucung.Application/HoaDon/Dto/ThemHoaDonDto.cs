using System;
using System.Collections.Generic;
namespace Cuahangchamsocthucung.HoaDon.Dto
{
    public class ThemHoaDonDto
    {
        public int LichChamSocId { get; set; }
    }

    public class ThemHoaDonChiTietDto
    {
        public int HoaDonId { get; set; }
        public int DichVuId { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}