using System;
using System.Collections.Generic;

public class ThemHoaDonDto
{
    public int LichChamSocId { get; set; }
    public int KhachHangId { get; set; }
    public int NhanVienId { get; set; }
    public DateTime NgayLap { get; set; }
    public decimal TongTien { get; set; }
    public string TrangThai { get; set; }
    public List<ThemHoaDonChiTietDto> ChiTietHoaDon { get; set; }
}

public class ThemHoaDonChiTietDto
{
    public int HoaDonId { get; set; }
    public int DichVuId { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}