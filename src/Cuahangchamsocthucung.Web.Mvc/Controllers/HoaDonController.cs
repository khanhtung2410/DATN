using Microsoft.AspNetCore.Mvc;
using Cuahangchamsocthucung.Web.Controllers;
using Cuahangchamsocthucung.Controllers;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class HoaDonController : CuahangchamsocthucungControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}