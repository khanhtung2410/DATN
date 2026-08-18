using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Web.Models.LichChamSoc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{

    public class LichChamSocController : CuahangchamsocthucungControllerBase
    {
        private readonly ILichChamSocAppService _lichChamSocAppService;

        public LichChamSocController(ILichChamSocAppService lichChamSocAppService)
        {
            _lichChamSocAppService = lichChamSocAppService;
        }

        [Authorize(Roles = StaticRoleNames.Tenants.Admin)]
        public async Task<IActionResult> Index()
        {
            var lichs = await _lichChamSocAppService.GetAll();
            var model = new LichChamSocListViewModel
            {
                LichChamSocs = lichs
            };
            return View(model);
        }
    }
}