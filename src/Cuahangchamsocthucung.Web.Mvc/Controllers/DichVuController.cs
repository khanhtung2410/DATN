using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Web.Models.DichVu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    
    [Authorize(Roles = StaticRoleNames.Tenants.Admin)]

    public class DichVuController : CuahangchamsocthucungControllerBase
    {
        private readonly IDichVuAppService _dichVuAppService;

        public DichVuController(IDichVuAppService dichVuAppService)
        {
            _dichVuAppService = dichVuAppService;
        }

        public async Task<IActionResult> Index()
        {
            var dichVus = await _dichVuAppService.GetAll();
            var model = new DichVuListViewModel
            {
                DichVus = dichVus
            };
            return View(model);
        }
    }
}