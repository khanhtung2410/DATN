using Abp.Application.Services;
using Abp.Domain.Repositories;
using Cuahangchamsocthucung.BaoCao.Dto;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.BaoCao
{
    public class BaoCaoAppService : ApplicationService, IBaoCaoAppService
    {
        private readonly IRepository<Entities.HoaDon, int> _hoaDonRepository;
        private readonly IRepository<Entities.LichChamSoc, int> _lichChamSocRepository;
        private readonly IRepository<Entities.KhachHang, int> _khachHangRepository;
        private readonly IRepository<Entities.ThuCung, int> _thuCungRepository;
        private readonly IRepository<Entities.NhanVien, int> _nhanVienRepository;

        public BaoCaoAppService(
            IRepository<Entities.HoaDon, int> hoaDonRepository,
            IRepository<Entities.LichChamSoc, int> lichChamSocRepository,
            IRepository<Entities.KhachHang, int> khachHangRepository,
            IRepository<Entities.ThuCung, int> thuCungRepository,
            IRepository<Entities.NhanVien, int> nhanVienRepository)
        {
            _hoaDonRepository = hoaDonRepository;
            _lichChamSocRepository = lichChamSocRepository;
            _khachHangRepository = khachHangRepository;
            _thuCungRepository = thuCungRepository;
            _nhanVienRepository = nhanVienRepository;
        }

        public async Task<BaoCaoDto> GetBaoCao(BaoCaoFilterDto input)
        {
            // ==========================================
            // 1. XÁC ĐỊNH THÁNG BÁO CÁO
            // ==========================================

            var homNay = DateTime.Today;

            var thang = input?.Thang ?? homNay.Month;
            var nam = input?.Nam ?? homNay.Year;

            if (thang < 1 || thang > 12)
            {
                throw new Abp.UI.UserFriendlyException(
                    "Tháng không hợp lệ.");
            }

            if (nam < 2000 || nam > 2100)
            {
                throw new Abp.UI.UserFriendlyException(
                    "Năm không hợp lệ.");
            }

            var dauThang = new DateTime(nam, thang, 1);

            var dauThangSau = dauThang.AddMonths(1);

            // ==========================================
            // 2. QUERY HÓA ĐƠN
            // ==========================================

            var hoaDons = _hoaDonRepository
                .GetAll()
                .Where(x =>
                    !x.IsDeleted &&
                    x.NgayLap >= dauThang &&
                    x.NgayLap < dauThangSau);

            // ==========================================
            // 3. QUERY LỊCH CHĂM SÓC
            // ==========================================

            var lichs = _lichChamSocRepository
                .GetAll()
                .Where(x =>
                    !x.IsDeleted &&
                    x.ThoiGian >= dauThang &&
                    x.ThoiGian < dauThangSau);

            // ==========================================
            // 4. QUERY KHÁCH HÀNG
            // ==========================================

            var khachHangs = _khachHangRepository
                .GetAll();

            // ==========================================
            // 5. QUERY THÚ CƯNG
            // ==========================================

            var thuCungs = _thuCungRepository
                .GetAll()
                .Where(x => !x.IsDeleted);

            // ==========================================
            // 6. HÓA ĐƠN ĐÃ THANH TOÁN
            // ==========================================

            var daThanhToan = hoaDons
                .Where(x =>
                    x.TrangThai == "Đã thanh toán" ||
                    x.TrangThai == "DaThanhToan");

            // ==========================================
            // 7. HÓA ĐƠN CHƯA THANH TOÁN
            // ==========================================

            var chuaThanhToan = hoaDons
                .Where(x =>
                    x.TrangThai == "Chưa thanh toán" ||
                    x.TrangThai == "ChuaThanhToan");

            // ==========================================
            // 8. DOANH THU THÁNG
            // ==========================================

            var doanhThu = await daThanhToan
                .SumAsync(x => (decimal?)x.TongTien) ?? 0;

            // ==========================================
            // 9. CHI PHÍ LƯƠNG
            // ==========================================
            //
            // Chỉ tính nhân viên đang làm việc.
            //
            // Vì báo cáo được lọc theo 1 tháng
            // nên không nhân thêm số tháng.
            //
            // Ví dụ:
            //
            // NV1: 8 triệu
            // NV2: 7 triệu
            // NV3: 9 triệu
            //
            // Tổng chi phí lương = 24 triệu
            // ==========================================

            var tongChiPhiLuong = await _nhanVienRepository
                .GetAll()
                .Where(x => x.Trangthai)
                .SumAsync(x => (decimal?)x.Luong) ?? 0;

            // ==========================================
            // 10. DOANH THU HÔM NAY
            // ==========================================
            //
            // Chỉ hiển thị nếu tháng đang xem là
            // tháng hiện tại.
            //
            // Nếu xem tháng cũ => 0.
            // ==========================================

            decimal doanhThuHomNay = 0;

            if (thang == homNay.Month && nam == homNay.Year)
            {
                var ngayMai = homNay.AddDays(1);

                doanhThuHomNay = await daThanhToan
                    .Where(x =>
                        x.NgayLap >= homNay &&
                        x.NgayLap < ngayMai)
                    .SumAsync(x => (decimal?)x.TongTien) ?? 0;
            }

            // ==========================================
            // 11. LỢI NHUẬN
            // ==========================================

            var loiNhuan = doanhThu - tongChiPhiLuong;

            // ==========================================
            // 12. TẠO RESULT
            // ==========================================

            var result = new BaoCaoDto
            {
                // Doanh thu
                DoanhThuDaThanhToan = doanhThu,

                TongDoanhThu = doanhThu,

                DoanhThuHomNay = doanhThuHomNay,

                DoanhThuThangNay = doanhThu,

                // Chi phí
                TongChiPhiLuong = tongChiPhiLuong,

                // Lợi nhuận
                LoiNhuan = loiNhuan,

                // Hóa đơn
                TongHoaDon = await hoaDons.CountAsync(),

                HoaDonDaThanhToan = await daThanhToan.CountAsync(),

                HoaDonChuaThanhToan = await chuaThanhToan.CountAsync(),

                // Khách hàng
                TongKhachHang = await khachHangs.CountAsync(),

                // Thú cưng
                TongThuCung = await thuCungs.CountAsync(),

                // Lịch chăm sóc
                TongLichChamSoc = await lichs.CountAsync(),

                LichHoanThanh = await lichs
                    .CountAsync(x =>
                        x.TrangThai ==
                        TrangThaiLichChamSoc.HoanThanh),

                LichDangDienRa = await lichs
                    .CountAsync(x =>
                        x.TrangThai ==
                        TrangThaiLichChamSoc.DangDienRa),

                LichChoXacNhan = await lichs
                    .CountAsync(x =>
                        x.TrangThai ==
                        TrangThaiLichChamSoc.ChoXacNhan),

                LichDaXacNhan = await lichs
                    .CountAsync(x =>
                        x.TrangThai ==
                        TrangThaiLichChamSoc.DaXacNhan)
            };

            // ==========================================
            // 13. DOANH THU THEO NGÀY
            // ==========================================
            //
            // Lấy doanh thu từng ngày.
            //
            // Sau đó bổ sung những ngày không có
            // doanh thu = 0.
            //
            // Ví dụ tháng 8:
            //
            // 01/08 = 2 triệu
            // 02/08 = 0
            // 03/08 = 4 triệu
            // ...
            // 31/08 = 1 triệu
            // ==========================================

            var doanhThuTheoNgayDb = await daThanhToan
                .GroupBy(x => x.NgayLap.Date)
                .Select(g => new DoanhThuTheoNgayDto
                {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien)
                })
                .ToListAsync();

            var doanhThuTheoNgay =
                new List<DoanhThuTheoNgayDto>();

            var soNgayTrongThang =
                DateTime.DaysInMonth(nam, thang);

            for (int ngay = 1; ngay <= soNgayTrongThang; ngay++)
            {
                var ngayHienTai =
                    new DateTime(nam, thang, ngay);

                var item = doanhThuTheoNgayDb
                    .FirstOrDefault(x =>
                        x.Ngay.Date == ngayHienTai.Date);

                doanhThuTheoNgay.Add(
                    new DoanhThuTheoNgayDto
                    {
                        Ngay = ngayHienTai,

                        DoanhThu = item != null
                            ? item.DoanhThu
                            : 0
                    });
            }

            result.DoanhThuTheoNgay =
                doanhThuTheoNgay;

            // ==========================================
            // 14. DOANH THU THEO DỊCH VỤ
            // ==========================================

            result.DoanhThuTheoDichVu =
                await daThanhToan
                    .SelectMany(x => x.ChiTietHoaDons)
                    .GroupBy(x => new
                    {
                        x.DichVuId,

                        TenDichVu =
                            x.DichVu != null
                                ? x.DichVu.TenDichVu
                                : "Không xác định"
                    })
                    .Select(g => new DoanhThuDichVuDto
                    {
                        DichVuId = g.Key.DichVuId,

                        TenDichVu = g.Key.TenDichVu,

                        DoanhThu =
                            g.Sum(x => x.ThanhTien),

                        SoLuong =
                            g.Count()
                    })
                    .OrderByDescending(x => x.DoanhThu)
                    .ToListAsync();

            // ==========================================
            // 15. RETURN
            // ==========================================

            return result;
        }
    }
}