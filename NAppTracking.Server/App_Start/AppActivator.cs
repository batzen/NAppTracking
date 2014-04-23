﻿namespace NAppTracking.Server
{
    using System;
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
            CreateDatabase();
        }

        public static void Stop()
        {
            NinjectWebCommon.Stop();
        }

        private static void CreateDatabase()
        {
            var db = DependencyResolver.Current.GetService<EntitiesContext>();

            if (db.Database.Exists()
                && db.Database.CompatibleWithModel(false) == false)
            {
                // Just drop all tables multiple times. This way we can delete all tables regardless of PKs and FKs.
                for (var i = 0; i < 5; i++)
                {
                    db.Database.ExecuteSqlCommand("exec sp_MSforeachtable 'DROP TABLE ?'");   
                }                
            }

            var haveToCreateDemoDatabaseEntries = NinjectWebCommon.Kernel.Get<IAppConfiguration>().IsDemoEnabled
                                                  && (db.Database.CreateIfNotExists() || db.Users.Any() == false);

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
                var result = await userManager.CreateAsync(demoUser, "demodemo");
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