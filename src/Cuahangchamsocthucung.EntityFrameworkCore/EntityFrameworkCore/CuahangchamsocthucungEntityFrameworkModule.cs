using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Cuahangchamsocthucung.EntityFrameworkCore.Seed;

namespace Cuahangchamsocthucung.EntityFrameworkCore
{
    [DependsOn(
        typeof(CuahangchamsocthucungCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class CuahangchamsocthucungEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<CuahangchamsocthucungDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        CuahangchamsocthucungDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        CuahangchamsocthucungDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CuahangchamsocthucungEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
