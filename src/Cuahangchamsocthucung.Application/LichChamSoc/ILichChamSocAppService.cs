using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Cuahangchamsocthucung.LichChamSoc.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

    public interface ILichChamSocAppService : IApplicationService
    {
        Task<List<LichChamSocDto>> GetAll();
        Task<LichChamSocDto> GetLichChamSoc(int id);
        Task<int> Create(ThemLichChamSocDto input);
        Task Update(SuaLichChamSocDto input);
        Task ChangeStatus(SuaTrangThaiLichChamSocDto input);
    Task<List<LichChamSocDto>> GetLichChamSocCuaToi();
    Task<List<LichChamSocDto>> GetLichSuLichChamSocCuaToi();
}
