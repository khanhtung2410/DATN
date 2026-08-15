using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Cuahangchamsocthucung.DichVu.Dto;
using Cuahangchamsocthucung.Nhanvien.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INhanvienAppService : IApplicationService
{
    Task<NhanvienDto> GetNhanVien(int id);

    Task<List<NhanvienDto>> GetAll();

    Task<int> Create(ThemNhanVienDto input);

    Task Update(SuaNhanVienDto input);

    Task ChangeTrangThai(SuaTrangThaiNhanVienDto input);
}