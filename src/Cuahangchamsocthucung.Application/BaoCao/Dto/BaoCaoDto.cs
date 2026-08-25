using System;
using System.Collections.Generic;

namespace Cuahangchamsocthucung.BaoCao.Dto
{
    public class BaoCaoDto
    {
        // =========================
        // DOANH THU
        // =========================

        public decimal DoanhThuDaThanhToan { get; set; }

        public decimal TongDoanhThu { get; set; }

        public decimal DoanhThuHomNay { get; set; }

        public decimal DoanhThuThangNay { get; set; }

        // =========================
        // CHI PHÍ
        // =========================

        public decimal TongChiPhiLuong { get; set; }

        // =========================
        // LỢI NHUẬN
        // =========================

        public decimal LoiNhuan { get; set; }

        // =========================
        // HÓA ĐƠN
        // =========================

        public int TongHoaDon { get; set; }

        public int HoaDonDaThanhToan { get; set; }

        public int HoaDonChuaThanhToan { get; set; }

        // =========================
        // KHÁCH HÀNG
        // =========================

        public int TongKhachHang { get; set; }

        // =========================
        // THÚ CƯNG
        // =========================

        public int TongThuCung { get; set; }

        // =========================
        // LỊCH CHĂM SÓC
        // =========================

        public int TongLichChamSoc { get; set; }

        public int LichHoanThanh { get; set; }

        public int LichDangDienRa { get; set; }

        public int LichChoXacNhan { get; set; }

        public int LichDaXacNhan { get; set; }

        // =========================
        // BIỂU ĐỒ
        // =========================

        public List<DoanhThuTheoNgayDto> DoanhThuTheoNgay { get; set; }
            = new List<DoanhThuTheoNgayDto>();

        public List<DoanhThuDichVuDto> DoanhThuTheoDichVu { get; set; }
            = new List<DoanhThuDichVuDto>();
    }

    public class DoanhThuTheoNgayDto
    {
        public DateTime Ngay { get; set; }

        public decimal DoanhThu { get; set; }
    }

    public class DoanhThuDichVuDto
    {
        public int DichVuId { get; set; }

        public string TenDichVu { get; set; }

        public decimal DoanhThu { get; set; }

        public int SoLuong { get; set; }
    }

    public class BaoCaoFilterDto
    {
        public int? Thang { get; set; }

        public int? Nam { get; set; }
    }
}