namespace NAppTracking.Server
{
    using Microsoft.AspNet.Identity;
    using NAppTracking.Server.Configuration;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Services;
    using Ninject.Modules;
    using Ninject.Web.Common;

    public class AppModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            var configuration = new ConfigurationService();
            this.Bind<ConfigurationService>()
                .ToMethod(context => configuration);
            this.Bind<IAppConfiguration>()
                .ToMethod(context => configuration.CurrentAppConfiguration);

            this.Bind<EntitiesContext>()
                .To<EntitiesContext>()
                .InRequestScope();

            this.Bind<IUserStore<ApplicationUser>>()
                .To<ApplicationUserStore>()
                .InRequestScope();

            this.Bind<ApplicationUserManager>()
                .To<ApplicationUserManager>()
                .InRequestScope();

            this.Bind<IFileSystemService>()
                .To<FileSystemService>()
                .InSingletonScope();

            this.Bind<IFileStorageService>()
                .To<FileSystemFileStorageService>()
                .InSingletonScope();

            this.Bind<IExceptionReportFileStorageService>()
                .To<ExceptionReportFileStorageService>()
                .InSingletonScope();
        }
    }
}