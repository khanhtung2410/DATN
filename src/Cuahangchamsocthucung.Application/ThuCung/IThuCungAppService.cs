using Abp.Application.Services;
using Cuahangchamsocthucung.ThuCung.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public interface IThuCungAppService : IApplicationService
{
    // Lấy tất cả thú cưng
    Task<List<ThuCungDto>> GetAll();

    // Lấy thú cưng theo Id
    Task<ThuCungDto> Get(int id);

    // Lấy thú cưng của khách hàng đang đăng nhập
    Task<List<ThuCungDto>> GetCuaToi();

    // Thêm thú cưng
    Task<int> Create(ThemThuCungDto input);

    // Sửa thú cưng
    Task Update(SuaThuCungDto input);

    // Xóa thú cưng
    Task Delete(int id);
}

