using Abp.Application.Services;
using Cuahangchamsocthucung.MatHang.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMatHangAppService : IApplicationService
{
    public Task<List<MathangDto>> LayDanhSachMatHang();
    public Task<MathangDto> LayChiTietMatHang(int id);
    public Task<int> ThemMatHang(ThemmathangDto input);
    public Task SuaMatHang(SuamathangDto input);
    public Task SuaTrangThaiMatHang(SuaTrangThaiMatHangDto input);
}

