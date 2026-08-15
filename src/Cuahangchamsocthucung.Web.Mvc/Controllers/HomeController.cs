using Abp.AspNetCore.Mvc.Authorization;
using Cuahangchamsocthucung.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cuahangchamsocthucung.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : CuahangchamsocthucungControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
