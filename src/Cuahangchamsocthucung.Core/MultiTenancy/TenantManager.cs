using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.Editions;

namespace Cuahangchamsocthucung.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore)
        {
        }
    }
}
