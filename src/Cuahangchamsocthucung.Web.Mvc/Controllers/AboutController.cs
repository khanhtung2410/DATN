using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using Cuahangchamsocthucung.Controllers;

namespace Cuahangchamsocthucung.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : CuahangchamsocthucungControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
