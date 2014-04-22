namespace NAppTracking.Server
{
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
            Bind<ConfigurationService>()
                .ToMethod(context => configuration);
            Bind<IAppConfiguration>()
                .ToMethod(context => configuration.Current);

            this.Bind<IEntitiesContext>()
                .To<EntitiesContext>()
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