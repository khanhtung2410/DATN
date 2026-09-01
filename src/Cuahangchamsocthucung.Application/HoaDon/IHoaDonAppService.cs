using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Cuahangchamsocthucung.HoaDon.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IHoaDonAppService : IApplicationService
{
    Task<List<HoaDonDto>> LayDanhSachHoaDon();
    Task<XemChiTietHoaDonDto> GetChiTietAsync(int hoaDonId);
    Task<int> ThemHoaDon(ThemHoaDonDto input);
    Task SuaHoaDon(SuaHoaDonDto input);
    Task DoiTrangThaiHoaDon(DoiTrangThaiHoaDonDto input);
    Task <ThanhToanQrDto> TaoQrThanhToan(int hoaDonId);
}
