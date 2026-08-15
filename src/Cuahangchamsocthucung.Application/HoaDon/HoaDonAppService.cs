using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.HoaDon.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class HoaDonAppService :
    ApplicationService,
    IHoaDonAppService
{
    private readonly IRepository<HoaDon> _hoaDonRepository;

    public HoaDonAppService(
        IRepository<HoaDon> hoaDonRepository
       )
    {
        _hoaDonRepository = hoaDonRepository;
    }

    public Task DoiTrangThaiHoaDon(DoiTrangThaiHoaDonDto input)
    {
        throw new System.NotImplementedException();
    }

    public async Task<XemChiTietHoaDonDto> GetChiTietAsync(int hoaDonId)
    {
 var hoaDon = await _hoaDonRepository.GetAsync(hoaDonId);
        if (hoaDon == null)
        {
            throw new UserFriendlyException("Không tìm thấy hóa đơn với ID: " + hoaDonId);
        }
        var chiTietHoaDons = new List<HoaDonChiTietDto>();
        foreach (var chiTiet in hoaDon.ChiTietHoaDons)
        {
            chiTietHoaDons.Add(new HoaDonChiTietDto
            {
                Id = chiTiet.Id,
                DichVuId = chiTiet.DichVuId,
                DonGia = chiTiet.DonGia
            });
        }
        return new XemChiTietHoaDonDto
        {
            Id = hoaDon.Id,
            KhachHangId = hoaDon.KhachHangId,
            NhanVienId = hoaDon.NhanVienId,
            NgayLap = hoaDon.NgayLap,
            TongTien = hoaDon.TongTien,
            TrangThai = hoaDon.TrangThai,
            ChiTietHoaDons = chiTietHoaDons
        };
    }

    public async Task<List<HoaDonDto>> LayDanhSachHoaDon()
    {
        var hoaDons = await _hoaDonRepository.GetAllListAsync();
        return hoaDons.Select(h => new HoaDonDto
        {
            Id = h.Id,
            KhachHangId = h.KhachHangId,
            NhanVienId = h.NhanVienId,
            NgayLap = h.NgayLap,
            TongTien = h.TongTien,
            TrangThai = h.TrangThai
        }).ToList();
    }

    public Task SuaHoaDon(SuaHoaDonDto input)
    {
        throw new System.NotImplementedException();
    }

    public Task<int> ThemHoaDon(ThemHoaDonDto input)
    {
        throw new System.NotImplementedException();
    }
}