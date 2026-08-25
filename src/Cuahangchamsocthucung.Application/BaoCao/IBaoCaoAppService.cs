using Cuahangchamsocthucung.BaoCao.Dto;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.BaoCao
{
    public interface IBaoCaoAppService
    {
        Task<BaoCaoDto> GetBaoCao(BaoCaoFilterDto input);
    }
}