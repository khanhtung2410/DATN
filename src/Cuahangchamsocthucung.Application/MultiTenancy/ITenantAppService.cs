using Abp.Application.Services;
using Cuahangchamsocthucung.MultiTenancy.Dto;

namespace Cuahangchamsocthucung.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

