using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Cuahangchamsocthucung.Configuration;

namespace Cuahangchamsocthucung.Web.Host.Startup
{
    [DependsOn(
       typeof(CuahangchamsocthucungWebCoreModule))]
    public class CuahangchamsocthucungWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public CuahangchamsocthucungWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CuahangchamsocthucungWebHostModule).GetAssembly());
        }
    }
}
