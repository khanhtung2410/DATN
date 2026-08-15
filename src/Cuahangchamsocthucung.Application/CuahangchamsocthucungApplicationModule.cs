using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Cuahangchamsocthucung.Authorization;

namespace Cuahangchamsocthucung
{
    [DependsOn(
        typeof(CuahangchamsocthucungCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class CuahangchamsocthucungApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<CuahangchamsocthucungAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(CuahangchamsocthucungApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
