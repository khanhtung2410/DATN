using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Cuahangchamsocthucung.Configuration;
using Cuahangchamsocthucung.EntityFrameworkCore;
using Cuahangchamsocthucung.Migrator.DependencyInjection;

namespace Cuahangchamsocthucung.Migrator
{
    [DependsOn(typeof(CuahangchamsocthucungEntityFrameworkModule))]
    public class CuahangchamsocthucungMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public CuahangchamsocthucungMigratorModule(CuahangchamsocthucungEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(CuahangchamsocthucungMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                CuahangchamsocthucungConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CuahangchamsocthucungMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
