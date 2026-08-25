using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.LichChamSoc.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILichChamSocAppService : IApplicationService
{
    Task<PagedResultDto<LichChamSocDto>> GetAll(
    string tenKhachHang = "",
    TrangThaiLichChamSoc? trangThai = null,
    int page = 1,
    int pageSize = 10);
    Task<LichChamSocDto> GetLichChamSoc(int id);
    Task<int> Create(ThemLichChamSocDto input);
    Task Update(SuaLichChamSocDto input);
    Task ChangeStatus(SuaTrangThaiLichChamSocDto input);
    Task<List<LichChamSocDto>> GetLichChamSocCuaToi();
    Task<List<LichChamSocDto>> GetLichSuLichChamSocCuaToi();
    Task PhanCongNhanVien(int lichChamSocId, int nhanVienId);
    Task HuyLichChamSoc(int id);
    Task TuChoiLichChamSoc(int id);
    Task BatDauViec(int id);
    Task HoanThanhLichChamSoc(int id);
    Task<string> TuDongPhanCongNhanVien(int lichChamSocId);
    Task<List<LichChamSocTimelineDto>> GetTimelineTrongNgay(DateTime ngay);
}
