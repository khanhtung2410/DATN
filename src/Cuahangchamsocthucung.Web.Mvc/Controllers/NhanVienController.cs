using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Web.Models.NhanVien;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    [Authorize(Roles = StaticRoleNames.Tenants.Admin)]
    public class NhanVienController : CuahangchamsocthucungControllerBase
    {
        private readonly INhanvienAppService _nhanVienAppService;

        public NhanVienController(INhanvienAppService nhanVienAppService)
        {
            _nhanVienAppService = nhanVienAppService;
        }

        public async Task<IActionResult> Index()
        {
            var nhanViens = await _nhanVienAppService.GetAll();
            var model = new NhanVienListViewModel
            {
                NhanViens = nhanViens
            };
            return View(model);
        }

        public async Task<IActionResult> EditModal(int id)
        {
            var nhanVien = await _nhanVienAppService.GetNhanVien(id);
            return PartialView("~/Views/NhanVien/_EditModal.cshtml", nhanVien);
        }
    }
}