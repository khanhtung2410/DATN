using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Cuahangchamsocthucung.EntityFrameworkCore;
using Cuahangchamsocthucung.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Cuahangchamsocthucung.Web.Tests
{
    [DependsOn(
        typeof(CuahangchamsocthucungWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class CuahangchamsocthucungWebTestModule : AbpModule
    {
        public CuahangchamsocthucungWebTestModule(CuahangchamsocthucungEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CuahangchamsocthucungWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(CuahangchamsocthucungWebMvcModule).Assembly);
        }
    }
}