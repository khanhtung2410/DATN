using Abp.Authorization;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Landing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class LandingController : CuahangchamsocthucungControllerBase
    {
        private readonly LandingAppService _landingAppService;
        private readonly IThuCungAppService _thuCungAppService;
        private readonly ILichChamSocAppService _lichChamSocAppService;
        public LandingController(
            LandingAppService landingAppService, IThuCungAppService thuCungAppService, ILichChamSocAppService lichChamSocAppService)
        {
            _landingAppService = landingAppService;
            _thuCungAppService = thuCungAppService;
            _lichChamSocAppService = lichChamSocAppService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> KhachSan()
        {
            var bangGias = await _landingAppService
                .GetBangGia("Trông giữ thú cưng");

            return View(bangGias);
        }

        [AllowAnonymous]
        public async Task<IActionResult> TamSpa()
        {
            var bangGias = await _landingAppService
                .GetBangGia("Tắm Spa");

            return View(bangGias);
        }

        [AllowAnonymous]
        public async Task<IActionResult> CatTiaLong()
        {
            var bangGias = await _landingAppService
                .GetBangGia("Cắt tỉa lông");

            return View(bangGias);
        }
        [AbpAuthorize]
        public async Task<IActionResult> ThuCung()
        {
            var thuCungs = await _thuCungAppService.GetCuaToi();

            return View(thuCungs);
        }
        [AbpAuthorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> LichChamSoc()
        {
            var lichDangXuLy = await _lichChamSocAppService.GetLichChamSocCuaToi();
            var lichSu = await _lichChamSocAppService.GetLichSuLichChamSocCuaToi();

            ViewBag.LichDangXuLy = lichDangXuLy;
            ViewBag.LichSu = lichSu;

            return View();
        }
    }
}