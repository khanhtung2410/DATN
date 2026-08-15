using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Cuahangchamsocthucung.DichVu.Dto;
using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DichVuAppService :
    ApplicationService,
    IDichVuAppService
{
    private readonly IRepository<DichVu> _dichVuRepository;
    private readonly IRepository<BangGia> _bangGiaRepository;


    public DichVuAppService(
        IRepository<DichVu> dichVuRepository,
        IRepository<BangGia> bangGiaRepository
       )
    {
        _dichVuRepository = dichVuRepository;
        _bangGiaRepository = bangGiaRepository;
    }

    [UnitOfWork]
    public async Task<int> Create(ThemDichVuDto input)
    {
        if (input.Tendichvu == null || input.Tendichvu.Trim() == "")
        {
            throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin bắt buộc.");
        }

        var dichVu = new DichVu
        {
            TenDichVu = input.Tendichvu,
            MoTa = input.Mota,
            TrangThai = true
        };
        var createdDichVuId = await _dichVuRepository.InsertAndGetIdAsync(dichVu);
        if (input.BangGias == null || input.BangGias.Count == 0)
        {
            throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin bắt buộc.");
        }
        foreach (var bangGiaDto in input.BangGias)
        {
            if (bangGiaDto.Cannangtu < 0 || bangGiaDto.Cannangden < 0 || bangGiaDto.Cannangden <= bangGiaDto.Cannangtu)
            {
                throw new UserFriendlyException("Khoảng cân nặng không hợp lệ.");
            }
            if (bangGiaDto.Giadv <= 0)
            {
                throw new UserFriendlyException("Giá dịch vụ không hợp lệ.");
            }
            if (string.IsNullOrWhiteSpace(bangGiaDto.Loaithucung) || bangGiaDto.Giadv == null)
            {
                throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin bắt buộc.");
            }
            var existingBangGia = await _bangGiaRepository.FirstOrDefaultAsync(bg =>
                            bg.Loailong == bangGiaDto.Loailong &&
                            bg.Loaithucung == bangGiaDto.Loaithucung &&
                            bg.Cannangtu == bangGiaDto.Cannangtu &&
                            bg.Cannangden == bangGiaDto.Cannangden);
            if (existingBangGia != null)
            {
                throw new UserFriendlyException("Mức giá này đã tồn tại.");
            }
            var bangGia = new BangGia
            {
                DichVuId = createdDichVuId,
                Loailong = bangGiaDto.Loailong,
                Loaithucung = bangGiaDto.Loaithucung,
                Cannangtu = bangGiaDto.Cannangtu,
                Cannangden = bangGiaDto.Cannangden,
                Giadv = bangGiaDto.Giadv
            };
            await _bangGiaRepository.InsertAsync(bangGia);
        }
        return createdDichVuId;
    }

    [UnitOfWork]
    public async Task ChangeTrangThai(
        SuaTrangThaiDichVuDto input)
    {
        var dichVu = await _dichVuRepository.GetAsync(input.Id);

        dichVu.TrangThai = input.Trangthai;
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
                Loailong = bg.Loailong,
                Loaithucung = bg.Loaithucung,
                Cannangtu = bg.Cannangtu,
                Cannangden = bg.Cannangden,
                Giadv = bg.Giadv
            }).ToList()
        }).ToList());
    }

    [UnitOfWork]
    public async Task Update(SuaDichVuDto input)
    {
        var dichVu = await _dichVuRepository.GetAllIncluding(d => d.BangGias).FirstOrDefaultAsync(d => d.Id == input.Id);
        if (input.Tendichvu == null || input.Tendichvu.Trim() == "")
        {
            throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin bắt buộc.");
        }
        if (input.BangGias == null || input.BangGias.Count == 0)
        {
            throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin bắt buộc.");
        }
        //Update DichVu 
        dichVu.TenDichVu = input.Tendichvu;
        dichVu.MoTa = input.Mota;
        dichVu.TrangThai = input.Trangthai;

        var incomingBangGiasIds = input.BangGias.Select(bg => bg.Id).ToList();

        //Add or update BangGia
        foreach(var bangGiaInput in input.BangGias)
        {
            if (bangGiaInput.Cannangtu < 0 || bangGiaInput.Cannangden < 0 || bangGiaInput.Cannangden <= bangGiaInput.Cannangtu)
            {
                throw new UserFriendlyException("Khoảng cân nặng không hợp lệ.");
            }
            if (bangGiaInput.Giadv <= 0)
            {
                throw new UserFriendlyException("Giá dịch vụ không hợp lệ.");
            }
            if (string.IsNullOrWhiteSpace(bangGiaInput.Loaithucung) || bangGiaInput.Giadv == null)
            {
                throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin bắt buộc.");
            }
            var existingBangGia = dichVu.BangGias.FirstOrDefault(bg => bg.Id == bangGiaInput.Id);
            if (existingBangGia != null)
            {
                //Update existing BangGia
                existingBangGia.Loailong = bangGiaInput.Loailong;
                existingBangGia.Loaithucung = bangGiaInput.Loaithucung;
                existingBangGia.Cannangtu = bangGiaInput.Cannangtu;
                existingBangGia.Cannangden = bangGiaInput.Cannangden;
                existingBangGia.Giadv = bangGiaInput.Giadv;
            }
            else
            {
                //Add new BangGia
                var newBangGia = new BangGia
                {
                    DichVuId = dichVu.Id,
                    Loailong = bangGiaInput.Loailong,
                    Loaithucung = bangGiaInput.Loaithucung,
                    Cannangtu = bangGiaInput.Cannangtu,
                    Cannangden = bangGiaInput.Cannangden,
                    Giadv = bangGiaInput.Giadv
                };
                dichVu.BangGias.Add(newBangGia);
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
        var dichVuDto = new DichVuDto
        {
            Id = dichVu.Id,
            Tendichvu = dichVu.TenDichVu,
            Mota = dichVu.MoTa,
            Trangthai = dichVu.TrangThai,
            BangGias = dichVu.BangGias.Select(bg => new BangGiaDto
            {
                Id = bg.Id,
                Loailong = bg.Loailong,
                Loaithucung = bg.Loaithucung,
                Cannangtu = bg.Cannangtu,
                Cannangden = bg.Cannangden,
                Giadv = bg.Giadv
            }).ToList()
        };
        return dichVuDto;
    }
}