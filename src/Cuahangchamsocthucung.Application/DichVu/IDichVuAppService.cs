using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Cuahangchamsocthucung.DichVu.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDichVuAppService : IApplicationService
{
    Task<DichVuDto> GetDichVu(int id);

    Task<List<DichVuDto>> GetAll();

    Task<int> Create(ThemDichVuDto input);

    Task Update(SuaDichVuDto input);
    Task UpdateBangGia(SuaBangGiaDto input);

    Task ChangeTrangThai(SuaTrangThaiDichVuDto input);
}