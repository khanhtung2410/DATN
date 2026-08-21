using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Vip;
using Cuahangchamsocthucung.Web.Models.KhachHang;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    
    [Authorize(Roles = StaticRoleNames.Tenants.Admin)]
    public class VipController : CuahangchamsocthucungControllerBase
    {
        private readonly IVipAppService _vipAppService;

        public VipController(IVipAppService vipAppService)
        {
            _vipAppService = vipAppService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _vipAppService.LayDanhSachVip();
            return View(data);
        }
    }
}