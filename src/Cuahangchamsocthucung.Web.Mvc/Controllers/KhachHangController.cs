using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Web.Models.KhachHang;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    
    [Authorize(Roles = StaticRoleNames.Tenants.Admin)]

    public class KhachHangController : CuahangchamsocthucungControllerBase
    {
        private readonly IKhachHangAppService _khachHangAppService;

        public KhachHangController(IKhachHangAppService khachHangAppService)
        {
            _khachHangAppService = khachHangAppService;
        }

        public async Task<IActionResult> Index()
        {
            var khachHangs = await _khachHangAppService.GetAllKhachHangAsync();
            var model = new KhachHangListViewModel
            {
                KhachHangs = khachHangs
            };
            return View(model);
        }
    }
}