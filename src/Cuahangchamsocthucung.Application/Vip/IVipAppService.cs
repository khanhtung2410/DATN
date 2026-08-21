using Cuahangchamsocthucung.Vip.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Vip
{
    public interface IVipAppService
    {
        Task<List<VipDto>> LayDanhSachVip();
        Task<VipDto> GetAsync(int id);
        Task<int> ThemVip(ThemVipDto input);
        Task SuaVip(SuaVipDto input);
        Task XoaVip(int id);
        Task<List<CauHinhVipDto>> LayCauHinhVip(int vipId);
        Task<int> ThemCauHinhVip(ThemCauHinhVipDto input);
        Task SuaCauHinhVip(SuaCauHinhVipDto input);
        Task XoaCauHinhVip(int id);
        Task<decimal> LayPhanTramGiam(int capVip, DateTime ngay);
    }
}