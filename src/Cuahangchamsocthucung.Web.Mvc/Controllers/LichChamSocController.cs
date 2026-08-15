using Microsoft.AspNetCore.Mvc;
using Cuahangchamsocthucung.Controllers;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class LichChamSocController : CuahangchamsocthucungControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}