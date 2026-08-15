using System.Threading.Tasks;
using Abp.Application.Services;
using Cuahangchamsocthucung.Authorization.Accounts.Dto;

namespace Cuahangchamsocthucung.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
