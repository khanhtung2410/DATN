using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.DichVu.Dto;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
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

    [AbpAuthorize(PermissionNames.Pages_DichVu)]
    [UnitOfWork]
    public async Task<int> Create(ThemDichVuDto input)
    {
        if (input == null)
            throw new UserFriendlyException("Dữ liệu dịch vụ không hợp lệ.");

        if (string.IsNullOrWhiteSpace(input.Tendichvu))
            throw new UserFriendlyException("Vui lòng nhập tên dịch vụ.");

        if (!System.Enum.IsDefined(typeof(LoaiDichVu), input.LoaiDichVu))
            throw new UserFriendlyException("Loại dịch vụ không hợp lệ.");

        if (input.BangGias == null || !input.BangGias.Any())
            throw new UserFriendlyException("Vui lòng nhập ít nhất một bảng giá.");

        var loaiDichVu = (LoaiDichVu)input.LoaiDichVu;

        KiemTraTrungBangGia(input.BangGias);

        foreach (var bg in input.BangGias)
        {
            ValidateBangGia(
                bg.Cannangtu,
                bg.Cannangden,
                bg.Giadv,
                bg.ThoiGianPhut,
                bg.Loaithucung,
                bg.LoaiPhong,
                loaiDichVu);
        }

        var dichVu = new DichVu
        {
            TenDichVu = input.Tendichvu.Trim(),
            MoTa = input.Mota?.Trim(),
            LoaiDichVu = loaiDichVu,
            TrangThai = input.Trangthai
        };

        var dichVuId = await _dichVuRepository.InsertAndGetIdAsync(dichVu);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var bg in input.BangGias)
        {
            await _bangGiaRepository.InsertAsync(new BangGia
            {
                DichVuId = dichVuId,
                Loailong = bg.Loailong,
                Loaithucung = bg.Loaithucung,
                LoaiPhong = bg.LoaiPhong,
                Cannangtu = bg.Cannangtu,
                Cannangden = bg.Cannangden,
                Giadv = bg.Giadv,
                ThoiGianPhut = bg.ThoiGianPhut
            });
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        return dichVuId;
    }

    [AbpAuthorize(PermissionNames.Pages_DichVu)]
    [UnitOfWork]
    public async Task ChangeTrangThai(SuaTrangThaiDichVuDto input)
    {
        var dichVu = await _dichVuRepository.FirstOrDefaultAsync(input.Id);

        if (dichVu == null)
            throw new UserFriendlyException("Không tìm thấy dịch vụ.");

        dichVu.TrangThai = input.Trangthai;

        await _dichVuRepository.UpdateAsync(dichVu);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [UnitOfWork]
    public async Task<List<DichVuDto>> GetAll()
    {
        return await _dichVuRepository
            .GetAllIncluding(d => d.BangGias)
            .Select(d => new DichVuDto
            {
                Id = d.Id,
                Tendichvu = d.TenDichVu,
                Mota = d.MoTa,
                LoaiDichVu = d.LoaiDichVu,
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
            })
            .ToListAsync();
    }

    [AbpAuthorize(PermissionNames.Pages_DichVu)]
    [UnitOfWork]
    public async Task Update(SuaDichVuDto input)
    {
        if (input == null)
            throw new UserFriendlyException("Dữ liệu dịch vụ không hợp lệ.");

        var dichVu = await _dichVuRepository
            .GetAllIncluding(d => d.BangGias)
            .FirstOrDefaultAsync(d => d.Id == input.Id);

        if (dichVu == null)
            throw new UserFriendlyException("Không tìm thấy dịch vụ.");

        if (string.IsNullOrWhiteSpace(input.Tendichvu))
            throw new UserFriendlyException("Vui lòng nhập tên dịch vụ.");

        if (!System.Enum.IsDefined(typeof(LoaiDichVu), input.LoaiDichVu))
            throw new UserFriendlyException("Loại dịch vụ không hợp lệ.");

        if (input.BangGias == null || !input.BangGias.Any())
            throw new UserFriendlyException("Dịch vụ phải có ít nhất một bảng giá.");

        var loaiDichVu = (LoaiDichVu)input.LoaiDichVu;

        KiemTraTrungBangGia(input.BangGias);

        foreach (var bg in input.BangGias)
        {
            ValidateBangGia(
                bg.Cannangtu,
                bg.Cannangden,
                bg.Giadv,
                bg.ThoiGianPhut,
                bg.Loaithucung,
                bg.LoaiPhong,
                loaiDichVu);
        }

        dichVu.TenDichVu = input.Tendichvu.Trim();
        dichVu.MoTa = input.Mota?.Trim();
        dichVu.LoaiDichVu = loaiDichVu;
        dichVu.TrangThai = input.Trangthai;

        var idsInput = input.BangGias
            .Where(x => x.Id > 0)
            .Select(x => x.Id)
            .ToHashSet();

        foreach (var bangGiaCu in dichVu.BangGias.ToList())
        {
            if (!idsInput.Contains(bangGiaCu.Id))
                await _bangGiaRepository.DeleteAsync(bangGiaCu);
        }

        foreach (var bgInput in input.BangGias)
        {
            var bangGiaCu = dichVu.BangGias
                .FirstOrDefault(x => x.Id == bgInput.Id);

            if (bangGiaCu != null)
            {
                bangGiaCu.Loailong = bgInput.Loailong;
                bangGiaCu.Loaithucung = bgInput.Loaithucung;
                bangGiaCu.LoaiPhong = bgInput.LoaiPhong;
                bangGiaCu.Cannangtu = bgInput.Cannangtu;
                bangGiaCu.Cannangden = bgInput.Cannangden;
                bangGiaCu.Giadv = bgInput.Giadv;
                bangGiaCu.ThoiGianPhut = bgInput.ThoiGianPhut;
            }
            else
            {
                await _bangGiaRepository.InsertAsync(new BangGia
                {
                    DichVuId = dichVu.Id,
                    Loailong = bgInput.Loailong,
                    Loaithucung = bgInput.Loaithucung,
                    LoaiPhong = bgInput.LoaiPhong,
                    Cannangtu = bgInput.Cannangtu,
                    Cannangden = bgInput.Cannangden,
                    Giadv = bgInput.Giadv,
                    ThoiGianPhut = bgInput.ThoiGianPhut
                });
            }
        }

        await _dichVuRepository.UpdateAsync(dichVu);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [UnitOfWork]
    public async Task<DichVuDto> GetDichVu(int id)
    {
        var dichVu = await _dichVuRepository
            .GetAllIncluding(d => d.BangGias)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dichVu == null)
            throw new UserFriendlyException("Không tìm thấy dịch vụ.");

        return new DichVuDto
        {
            Id = dichVu.Id,
            Tendichvu = dichVu.TenDichVu,
            Mota = dichVu.MoTa,
            LoaiDichVu = dichVu.LoaiDichVu,
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

    [AbpAuthorize(PermissionNames.Pages_DichVu)]
    [UnitOfWork]
    public async Task UpdateBangGia(SuaBangGiaDto input)
    {
        var bangGia = await _bangGiaRepository.GetAsync(input.Id);

        if (bangGia.DichVuId != input.DichVuId)
            throw new UserFriendlyException("Bảng giá không thuộc dịch vụ này.");

        var dichVu = await _dichVuRepository.FirstOrDefaultAsync(x => x.Id == input.DichVuId);

        if (dichVu == null)
            throw new UserFriendlyException("Không tìm thấy dịch vụ.");

        ValidateBangGia(
            input.Cannangtu,
            input.Cannangden,
            input.Giadv,
            input.ThoiGianPhut,
            input.Loaithucung,
            input.LoaiPhong,
            dichVu.LoaiDichVu);

        var trungBangGia = await _bangGiaRepository.GetAll()
            .AnyAsync(bg =>
                bg.Id != input.Id &&
                bg.DichVuId == input.DichVuId &&
                bg.Loailong == input.Loailong &&
                bg.Loaithucung == input.Loaithucung &&
                bg.LoaiPhong == input.LoaiPhong &&
                bg.Cannangtu == input.Cannangtu &&
                bg.Cannangden == input.Cannangden);

        if (trungBangGia)
            throw new UserFriendlyException("Mức giá này đã tồn tại.");

        bangGia.Loailong = input.Loailong;
        bangGia.Loaithucung = input.Loaithucung;
        bangGia.LoaiPhong = input.LoaiPhong;
        bangGia.Cannangtu = input.Cannangtu;
        bangGia.Cannangden = input.Cannangden;
        bangGia.Giadv = input.Giadv;
        bangGia.ThoiGianPhut = input.ThoiGianPhut;

        await CurrentUnitOfWork.SaveChangesAsync();
    }

    private void ValidateBangGia(int canNangTu, int canNangDen, decimal gia, int thoiGianPhut, string loaiThuCung, string loaiPhong, LoaiDichVu loaiDichVu)
    {
        if (string.IsNullOrWhiteSpace(loaiThuCung))
            throw new UserFriendlyException("Vui lòng nhập đối tượng.");

        if (canNangTu < 0 || canNangDen <= canNangTu)
            throw new UserFriendlyException("Khoảng cân nặng không hợp lệ.");

        if (gia <= 0)
            throw new UserFriendlyException("Giá dịch vụ không hợp lệ.");

        if (thoiGianPhut < 1 || thoiGianPhut > 1440)
            throw new UserFriendlyException("Thời gian phải từ 1 đến 1440 phút.");

        if (loaiDichVu == LoaiDichVu.LuuTru && string.IsNullOrWhiteSpace(loaiPhong))
            throw new UserFriendlyException("Vui lòng chọn loại phòng cho dịch vụ lưu trú.");

        if (loaiDichVu == LoaiDichVu.ChamSoc && !string.IsNullOrWhiteSpace(loaiPhong))
            throw new UserFriendlyException("Dịch vụ chăm sóc không được nhập loại phòng.");
    }

    private void KiemTraTrungBangGia(IEnumerable<ThemBangGiaDto> bangGias)
    {
        var trung = bangGias
            .GroupBy(x => new
            {
                x.Loaithucung,
                x.LoaiPhong,
                x.Loailong,
                x.Cannangtu,
                x.Cannangden
            })
            .FirstOrDefault(x => x.Count() > 1);

        if (trung != null)
            throw new UserFriendlyException("Không được nhập trùng mức giá.");
    }

    private void KiemTraTrungBangGia(IEnumerable<SuaBangGiaDto> bangGias)
    {
        var trung = bangGias
            .GroupBy(x => new
            {
                x.Loaithucung,
                x.LoaiPhong,
                x.Loailong,
                x.Cannangtu,
                x.Cannangden
            })
            .FirstOrDefault(x => x.Count() > 1);

        if (trung != null)
            throw new UserFriendlyException("Không được nhập trùng mức giá.");
    }
}