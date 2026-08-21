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
        public async Task<ActionResult> Index(string tenKhachHang = "", TrangThaiLichChamSoc? trangThai = null, int page = 1)
        {
            var lichs = await _lichChamSocAppService.GetAll() ?? new List<LichChamSocDto>();

            if (!string.IsNullOrWhiteSpace(tenKhachHang))
            {
                tenKhachHang = tenKhachHang.Trim();
                lichs = lichs.Where(x => !string.IsNullOrEmpty(x.TenKhachHang) && x.TenKhachHang.Contains(tenKhachHang, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (trangThai.HasValue)
                lichs = lichs.Where(x => x.TrangThai == trangThai.Value).ToList();

            const int pageSize = 10;
            var totalItems = lichs.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1)
                page = 1;

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var pagedLichs = lichs
                .OrderByDescending(x => x.ThoiGian)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new LichChamSocListViewModel
            {
                LichChamSocs = pagedLichs,
                TenKhachHang = tenKhachHang,
                TrangThai = trangThai,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }
    }
}