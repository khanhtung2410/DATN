using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Cuahangchamsocthucung.DichVu.Dto;
using Cuahangchamsocthucung.NhanVien.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INhanvienAppService : IApplicationService
{
    Task<NhanVienDto> GetNhanVien(int id);

    Task<List<NhanVienDto>> GetAll();

    Task<int> Create(ThemNhanVienDto input);

    Task Update(SuaNhanVienDto input);

    Task ChangeTrangThai(SuaTrangThaiNhanVienDto input);
    Task<List<NhanVienDto>> GetNhanVienDangLamViec();
}