using Microsoft.AspNetCore.Mvc;
using Cuahangchamsocthucung.Controllers;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class KhachHangController : CuahangchamsocthucungControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}