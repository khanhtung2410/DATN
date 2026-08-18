using Microsoft.AspNetCore.Mvc;
using Cuahangchamsocthucung.Web.Controllers;
using Cuahangchamsocthucung.Controllers;
using System.Threading.Tasks;
using Cuahangchamsocthucung.Web.Models.HoaDon;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class HoaDonController : CuahangchamsocthucungControllerBase
    {
        private readonly IHoaDonAppService _hoaDonAppService;

        public HoaDonController(IHoaDonAppService hoaDonAppService)
        {
            _hoaDonAppService = hoaDonAppService;
        }

        public async Task<IActionResult> Index()
        {
            var hoaDons = await _hoaDonAppService.LayDanhSachHoaDon();
            var model = new HoaDonListViewModel
            {
                HoaDons = hoaDons
            };
            return View(model);
        }
    }
}