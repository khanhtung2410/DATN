using Abp.Application.Services;
using Cuahangchamsocthucung.MatHang.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMatHangAppService : IApplicationService
{
    public Task<List<MatHangDto>> LayDanhSachMatHang();
    public Task<MatHangDto> LayChiTietMatHang(int id);
    public Task<int> ThemMatHang(ThemMatHangDto input);
    public Task SuaMatHang(SuaMatHangDto input);
    public Task SuaTrangThaiMatHang(SuaTrangThaiMatHangDto input);
}

