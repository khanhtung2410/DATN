using Abp.AutoMapper;
using Cuahangchamsocthucung.Sessions.Dto;

namespace Cuahangchamsocthucung.Web.Views.Shared.Components.TenantChange
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}
