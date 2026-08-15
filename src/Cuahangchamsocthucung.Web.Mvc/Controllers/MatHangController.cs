using Microsoft.AspNetCore.Mvc;
using Cuahangchamsocthucung.Controllers;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class MatHangController : CuahangchamsocthucungControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}