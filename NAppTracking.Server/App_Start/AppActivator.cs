namespace NAppTracking.Server
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using AutoMapper;
    using NAppTracking.Client;
    using NAppTracking.Server.Configuration;
    using NAppTracking.Server.Entities;
    using Ninject;

    public class AppActivator
    {
        public static void PreStart()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            NinjectWebCommon.Start();

            Mapper.CreateMap<ExceptionReportDto, ExceptionReport>();
            Mapper.CreateMap<ExceptionReportCustomDataSetDto, ExceptionReportCustomDataSet>();
            Mapper.CreateMap<ExceptionReportCustomDataDto, ExceptionReportCustomData>();
        }

        public static void PostStart()
        {
            if (NinjectWebCommon.Kernel.Get<IAppConfiguration>().IsDemoEnabled)
            {
                CreateDemoDatabase();
            }
        }

        public static void Stop()
        {
            NinjectWebCommon.Stop();
        }

        private static void CreateDemoDatabase()
        {
            var db = DependencyResolver.Current.GetService<EntitiesContext>();

            if (db.Database.Exists()
                && db.Database.CompatibleWithModel(false) == false)
            {
                if (db.Database.Connection.State != ConnectionState.Open)
                {
                    db.Database.Connection.Open();
                }

                // Just drop all tables multiple times. This way we can delete all tables regardless of PKs and FKs.
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        using (var command = db.Database.Connection.CreateCommand())
                        {
                            command.CommandText = "exec sp_MSforeachtable 'DROP TABLE ?'";
                            command.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                    }                    
                }                
            }

            var haveToCreateDemoDatabaseEntries = db.Database.CreateIfNotExists() || db.Users.Any() == false;

            if (haveToCreateDemoDatabaseEntries)
            {
                CreateDemoDatabaseEntries(db);
            }
        }

        private static async void CreateDemoDatabaseEntries(EntitiesContext db)
        {
            var demoUser = db.Users.Create();
            demoUser.UserName = "demo";

            using (var userManager = DependencyResolver.Current.GetService<ApplicationUserManager>())
            {
                var result = await userManager.CreateAsync(demoUser, "demopassword");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors));
                }
            }

            var demoApplication = db.TrackingApplications.Create();
            demoApplication.Name = "Demo Application";
            demoApplication.ApiKey = Guid.Empty;
            demoApplication.Owners.Add(demoUser);
            db.TrackingApplications.Add(demoApplication);

            await db.SaveChangesAsync();
        }
    }
}