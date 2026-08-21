using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Cuahangchamsocthucung.DichVu.Dto;
using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DichVuAppService : ApplicationService, IDichVuAppService
{
    private readonly IRepository<DichVu> _dichVuRepository;
    private readonly IRepository<BangGia> _bangGiaRepository;

    public DichVuAppService(IRepository<DichVu> dichVuRepository, IRepository<BangGia> bangGiaRepository)
    {
        _dichVuRepository = dichVuRepository;
        _bangGiaRepository = bangGiaRepository;
    }

    [UnitOfWork]
    public async Task<int> Create(ThemDichVuDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Tendichvu))
            throw new UserFriendlyException("Vui lòng nhập tên dịch vụ.");
        if (input.BangGias == null || input.BangGias.Count == 0)
            throw new UserFriendlyException("Vui lòng nhập ít nhất một bảng giá.");

        var dichVu = new DichVu { TenDichVu = input.Tendichvu.Trim(), MoTa = input.Mota, TrangThai = true };
        var createdDichVuId = await _dichVuRepository.InsertAndGetIdAsync(dichVu);

        foreach (var bangGiaDto in input.BangGias)
        {
            ValidateBangGia(bangGiaDto.Cannangtu, bangGiaDto.Cannangden, bangGiaDto.Giadv, bangGiaDto.ThoiGianPhut, bangGiaDto.Loaithucung, bangGiaDto.LoaiPhong, input.Tendichvu);

            var existingBangGia = await _bangGiaRepository.FirstOrDefaultAsync(bg =>
                bg.DichVuId == createdDichVuId &&
                bg.Loailong == bangGiaDto.Loailong &&
                bg.Loaithucung == bangGiaDto.Loaithucung &&
                bg.LoaiPhong == bangGiaDto.LoaiPhong &&
                bg.Cannangtu == bangGiaDto.Cannangtu &&
                bg.Cannangden == bangGiaDto.Cannangden);

            if (existingBangGia != null)
                throw new UserFriendlyException("Mức giá này đã tồn tại.");

            await _bangGiaRepository.InsertAsync(new BangGia
            {
                DichVuId = createdDichVuId,
                Loailong = bangGiaDto.Loailong,
                Loaithucung = bangGiaDto.Loaithucung,
                LoaiPhong = bangGiaDto.LoaiPhong,
                Cannangtu = bangGiaDto.Cannangtu,
                Cannangden = bangGiaDto.Cannangden,
                Giadv = bangGiaDto.Giadv,
                ThoiGianPhut = bangGiaDto.ThoiGianPhut
            });
        }

        return createdDichVuId;
    }

    [UnitOfWork]
    public async Task ChangeTrangThai(SuaTrangThaiDichVuDto input)
    {
        var dichVu = await _dichVuRepository.FirstOrDefaultAsync(input.Id);
        if (dichVu == null) throw new UserFriendlyException("Không tìm thấy dịch vụ.");
        dichVu.TrangThai = input.Trangthai;
        await _dichVuRepository.UpdateAsync(dichVu);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [UnitOfWork]
    public Task<List<DichVuDto>> GetAll()
    {
        var query = _dichVuRepository.GetAllIncluding(d => d.BangGias);
        return Task.FromResult(query.Select(d => new DichVuDto
        {
            Id = d.Id,
            Tendichvu = d.TenDichVu,
            Mota = d.MoTa,
            Trangthai = d.TrangThai,
            BangGias = d.BangGias.Select(bg => new BangGiaDto
            {
                Id = bg.Id,
                DichvuId = bg.DichVuId,
                Loailong = bg.Loailong,
                Loaithucung = bg.Loaithucung,
                LoaiPhong = bg.LoaiPhong,
                Cannangtu = bg.Cannangtu,
                Cannangden = bg.Cannangden,
                Giadv = bg.Giadv,
                ThoiGianPhut = bg.ThoiGianPhut
            }).ToList()
        }).ToList());
    }

    [UnitOfWork]
    public async Task Update(SuaDichVuDto input)
    {
        var dichVu = await _dichVuRepository.GetAllIncluding(d => d.BangGias).FirstOrDefaultAsync(d => d.Id == input.Id);
        if (dichVu == null) throw new UserFriendlyException("Không tìm thấy dịch vụ.");
        if (string.IsNullOrWhiteSpace(input.Tendichvu)) throw new UserFriendlyException("Vui lòng nhập tên dịch vụ.");
        if (input.BangGias == null || input.BangGias.Count == 0) throw new UserFriendlyException("Vui lòng nhập ít nhất một bảng giá.");

        dichVu.TenDichVu = input.Tendichvu.Trim();
        dichVu.MoTa = input.Mota;
        dichVu.TrangThai = input.Trangthai;

        foreach (var bangGiaInput in input.BangGias)
        {
            ValidateBangGia(bangGiaInput.Cannangtu, bangGiaInput.Cannangden, bangGiaInput.Giadv, bangGiaInput.ThoiGianPhut, bangGiaInput.Loaithucung, bangGiaInput.LoaiPhong, input.Tendichvu);

            var existingBangGia = dichVu.BangGias.FirstOrDefault(bg => bg.Id == bangGiaInput.Id);
            if (existingBangGia != null)
            {
                existingBangGia.Loailong = bangGiaInput.Loailong;
                existingBangGia.Loaithucung = bangGiaInput.Loaithucung;
                existingBangGia.LoaiPhong = bangGiaInput.LoaiPhong;
                existingBangGia.Cannangtu = bangGiaInput.Cannangtu;
                existingBangGia.Cannangden = bangGiaInput.Cannangden;
                existingBangGia.Giadv = bangGiaInput.Giadv;
                existingBangGia.ThoiGianPhut = bangGiaInput.ThoiGianPhut;
            }
            else
            {
                dichVu.BangGias.Add(new BangGia
                {
                    DichVuId = dichVu.Id,
                    Loailong = bangGiaInput.Loailong,
                    Loaithucung = bangGiaInput.Loaithucung,
                    LoaiPhong = bangGiaInput.LoaiPhong,
                    Cannangtu = bangGiaInput.Cannangtu,
                    Cannangden = bangGiaInput.Cannangden,
                    Giadv = bangGiaInput.Giadv,
                    ThoiGianPhut = bangGiaInput.ThoiGianPhut
                });
            }
        }

        await _dichVuRepository.UpdateAsync(dichVu);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [UnitOfWork]
    public async Task<DichVuDto> GetDichVu(int id)
    {
        var dichVu = await _dichVuRepository.GetAllIncluding(d => d.BangGias).FirstOrDefaultAsync(d => d.Id == id);
        if (dichVu == null) throw new UserFriendlyException("Không tìm thấy dịch vụ.");

        return new DichVuDto
        {
            Id = dichVu.Id,
            Tendichvu = dichVu.TenDichVu,
            Mota = dichVu.MoTa,
            Trangthai = dichVu.TrangThai,
            BangGias = dichVu.BangGias.Select(bg => new BangGiaDto
            {
                Id = bg.Id,
                DichvuId = bg.DichVuId,
                Loailong = bg.Loailong,
                Loaithucung = bg.Loaithucung,
                LoaiPhong = bg.LoaiPhong,
                Cannangtu = bg.Cannangtu,
                Cannangden = bg.Cannangden,
                Giadv = bg.Giadv,
                ThoiGianPhut = bg.ThoiGianPhut
            }).ToList()
        };
    }

    [UnitOfWork]
    public async Task UpdateBangGia(SuaBangGiaDto input)
    {
        var bangGia = await _bangGiaRepository.GetAsync(input.Id);
        ValidateBangGia(input.Cannangtu, input.Cannangden, input.Giadv, input.ThoiGianPhut, input.Loaithucung, input.LoaiPhong, null);

        bangGia.Loailong = input.Loailong;
        bangGia.Loaithucung = input.Loaithucung;
        bangGia.LoaiPhong = input.LoaiPhong;
        bangGia.Cannangtu = input.Cannangtu;
        bangGia.Cannangden = input.Cannangden;
        bangGia.Giadv = input.Giadv;
        bangGia.ThoiGianPhut = input.ThoiGianPhut;

        await CurrentUnitOfWork.SaveChangesAsync();
    }

    private void ValidateBangGia(int canNangTu, int canNangDen, decimal gia, int thoiGianPhut, string loaiThuCung, string loaiPhong, string tenDichVu)
    {
        if (string.IsNullOrWhiteSpace(loaiThuCung))
            throw new UserFriendlyException("Vui lòng nhập đối tượng.");
        if (canNangTu < 0 || canNangDen <= canNangTu)
            throw new UserFriendlyException("Khoảng cân nặng không hợp lệ.");
        if (gia <= 0)
            throw new UserFriendlyException("Giá dịch vụ không hợp lệ.");
        if (thoiGianPhut < 1 || thoiGianPhut > 1440)
            throw new UserFriendlyException("Thời gian phải từ 1 đến 1440 phút.");

        if (!string.IsNullOrWhiteSpace(tenDichVu) && tenDichVu.Trim() != "Trông giữ thú cưng" && !string.IsNullOrWhiteSpace(loaiPhong))
            throw new UserFriendlyException("Chỉ dịch vụ Trông giữ thú cưng mới được nhập loại phòng.");

        if (tenDichVu?.Trim() == "Trông giữ thú cưng" && string.IsNullOrWhiteSpace(loaiPhong))
            throw new UserFriendlyException("Vui lòng chọn loại phòng cho dịch vụ Trông giữ thú cưng.");
    }
}