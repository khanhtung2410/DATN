using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Cuahangchamsocthucung.KhachHang.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IKhachHangAppService : IApplicationService
{
    Task<List<KhachHangDto>> GetAllKhachHangAsync();
    Task<KhachHangDto> GetKhachHangByIdAsync(int id);
    Task<KhachHangDto> DangKy(DangKyDto input);
    Task<KhachHangDto> GetThongTinCaNhanAsync();
}