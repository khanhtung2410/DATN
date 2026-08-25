using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.Web.Models.LichChamSoc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuahangchamsocthucung.LichChamSoc.Dto;
namespace Cuahangchamsocthucung.Web.Controllers
{

    public class LichChamSocController : CuahangchamsocthucungControllerBase
    {
        private readonly ILichChamSocAppService _lichChamSocAppService;

        public LichChamSocController(ILichChamSocAppService lichChamSocAppService)
        {
            _lichChamSocAppService = lichChamSocAppService;
        }

        [Authorize(Roles = StaticRoleNames.Tenants.Admin)]
        public async Task<ActionResult> Index(
    string tenKhachHang = "",
    TrangThaiLichChamSoc? trangThai = null,
    DateTime? tuNgay = null,
    DateTime? denNgay = null,
    int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
                page = 1;

            var result = await _lichChamSocAppService.GetAll(
                tenKhachHang,
                trangThai,
                page,
                pageSize);

            var model = new LichChamSocListViewModel
            {
                LichChamSocs = result.Items.ToList(),
                TenKhachHang = tenKhachHang,
                TrangThai = trangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize)
            };

            return View(model);
        }

        [Authorize(Roles = StaticRoleNames.Tenants.Admin)]
        public async Task<IActionResult> Timeline(DateTime? ngay, int? nhanVienId)
        {
            var ngayXem = ngay?.Date ?? DateTime.Today;

            var lichChamSocs =
                await _lichChamSocAppService.GetTimelineTrongNgay(ngayXem);

            if (nhanVienId.HasValue)
            {
                lichChamSocs = lichChamSocs
                    .Where(x => x.NhanVienId == nhanVienId.Value)
                    .ToList();
            }

            var model = new LichChamSocTimelineViewModel
            {
                Ngay = ngayXem,
                NhanVienId = nhanVienId,
                LichChamSocs = lichChamSocs
            };

            return View(model);
        }
    }
}