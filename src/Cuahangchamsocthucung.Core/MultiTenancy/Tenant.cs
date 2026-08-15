using Abp.MultiTenancy;
using Cuahangchamsocthucung.Authorization.Users;

namespace Cuahangchamsocthucung.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
