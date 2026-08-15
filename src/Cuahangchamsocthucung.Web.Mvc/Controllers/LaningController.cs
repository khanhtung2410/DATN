using Cuahangchamsocthucung.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class LandingController : CuahangchamsocthucungControllerBase
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}