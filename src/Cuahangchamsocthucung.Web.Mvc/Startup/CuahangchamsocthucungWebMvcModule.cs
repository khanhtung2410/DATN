using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Cuahangchamsocthucung.Configuration;

namespace Cuahangchamsocthucung.Web.Startup
{
    [DependsOn(typeof(CuahangchamsocthucungWebCoreModule))]
    public class CuahangchamsocthucungWebMvcModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public CuahangchamsocthucungWebMvcModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Navigation.Providers.Add<CuahangchamsocthucungNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CuahangchamsocthucungWebMvcModule).GetAssembly());
        }
    }
}
