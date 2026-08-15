using Cuahangchamsocthucung.MatHang.Dto;
using Cuahangchamsocthucung.Entities;
using Abp.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using Abp.UI;

public class MatHangAppService :
    ApplicationService,
    IMatHangAppService
{
    private readonly IRepository<MatHang> _matHangRepository;

    public MatHangAppService(
      IRepository<MatHang> matHangRepository
    )
    {
        _matHangRepository = matHangRepository;
    }
    public async Task<List<MathangDto>> LayDanhSachMatHang()
    {
        var matHangs = await _matHangRepository.GetAll().ToListAsync();
        return ObjectMapper.Map<List<MathangDto>>(matHangs);
    }
    public async Task<MathangDto> LayChiTietMatHang(int id)
    {
        var matHang = await _matHangRepository.GetAsync(id);
        return ObjectMapper.Map<MathangDto>(matHang);
    }
    public async Task<int> ThemMatHang(ThemmathangDto input)
    {
        var matHang = ObjectMapper.Map<MatHang>(input);
        if (matHang.Soluong < 0)
        {
            throw new UserFriendlyException("Số lượng không được âm.");
        }
        var createdMatHangId = await _matHangRepository.InsertAndGetIdAsync(matHang);
        return createdMatHangId;
    }
    public async Task SuaMatHang(SuamathangDto input)
    {
        var matHang = await _matHangRepository.GetAsync(input.Id);
        if (matHang == null)
        {
            throw new UserFriendlyException("Mặt hàng không tồn tại.");
        }
        if (input.Soluong < 0)
        {
            throw new UserFriendlyException("Số lượng không được âm.");
        }
        ObjectMapper.Map(input, matHang);
        await _matHangRepository.UpdateAsync(matHang);
    }
    public async Task SuaTrangThaiMatHang(SuaTrangThaiMatHangDto input)
    {
        var matHang = await _matHangRepository.GetAsync(input.Id);
        matHang.Trangthai = input.Trangthai;
        await _matHangRepository.UpdateAsync(matHang);
    }

}