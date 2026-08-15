using System.Threading.Tasks;
using Abp.Application.Services;
using Cuahangchamsocthucung.Sessions.Dto;

namespace Cuahangchamsocthucung.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
