using Abp.AspNetCore.Mvc.Authorization;
using Abp.UI;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.HoaDon;
using Cuahangchamsocthucung.Web.Models.HoaDon;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HoaDonController : CuahangchamsocthucungControllerBase
    {
        private readonly IHoaDonAppService _hoaDonAppService;

        public HoaDonController(IHoaDonAppService hoaDonAppService)
        {
            _hoaDonAppService = hoaDonAppService;
        }

        public async Task<IActionResult> Index(string tenKhachHang, string trangThai, DateTime? tuNgay, DateTime? denNgay, int page = 1)
        {
            const int pageSize = 10;
            var hoaDons = await _hoaDonAppService.LayDanhSachHoaDon();

            if (!string.IsNullOrWhiteSpace(tenKhachHang))
            {
                var keyword = tenKhachHang.Trim();
                hoaDons = hoaDons.Where(x =>
                    !string.IsNullOrEmpty(x.TenKhachHang) &&
                    x.TenKhachHang.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                hoaDons = hoaDons.Where(x => x.TrangThai == trangThai).ToList();
            }

            if (tuNgay.HasValue)
            {
                var from = tuNgay.Value.Date;
                hoaDons = hoaDons.Where(x => x.NgayLap >= from).ToList();
            }

            if (denNgay.HasValue)
            {
                var to = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                hoaDons = hoaDons.Where(x => x.NgayLap <= to).ToList();
            }

            hoaDons = hoaDons.OrderByDescending(x => x.NgayLap).ToList();

            var totalItems = hoaDons.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));

            page = Math.Max(1, Math.Min(page, totalPages));

            var items = hoaDons
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new HoaDonListViewModel
            {
                HoaDons = items,
                TenKhachHang = tenKhachHang,
                TrangThai = trangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0)
                return NotFound();

            var hoaDon = await _hoaDonAppService.GetChiTietAsync(id);

            if (hoaDon == null)
                return NotFound();

            return PartialView("_ChiTiet", hoaDon);
        }

        public async Task<IActionResult> InHoaDon(int id)
        {
            var hoaDon = await _hoaDonAppService.GetChiTietAsync(id);

            if (hoaDon.TrangThai != "DaThanhToan" && hoaDon.TrangThai != "Đã thanh toán")
                throw new UserFriendlyException("Chỉ có thể in hóa đơn sau khi thanh toán.");

            return View("InHoaDon", hoaDon);
        }
    }
}