using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cuahangchamsocthucung.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : CuahangchamsocthucungControllerBase
    {
        
        [Authorize(Roles = StaticRoleNames.Tenants.Admin)]

        public ActionResult Index()
        {
            return View();
        }
    }
}
