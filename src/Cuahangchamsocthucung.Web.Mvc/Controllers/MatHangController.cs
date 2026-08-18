using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Web.Models.MatHang;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    
    [Authorize(Roles = StaticRoleNames.Tenants.Admin)]

    public class MatHangController : CuahangchamsocthucungControllerBase
    {
        private readonly IMatHangAppService _matHangAppService;

        public MatHangController(IMatHangAppService matHangAppService)
        {
            _matHangAppService = matHangAppService;
        }

        public async Task<IActionResult> Index()
        {
            var matHangs = await _matHangAppService.LayDanhSachMatHang();
            var model = new MatHangListViewModel
            {
                MatHangs = matHangs
            };
            return View(model);
        }
    }
}