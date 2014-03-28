﻿namespace NAppTracking.Server
{
    using System;
    using System.Security.Claims;
    using System.Web.Helpers;
    using Microsoft.Ajax.Utilities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using NAppTracking.Server.Entities;
    using Ninject;

    public class AppActivator
    {
        public static void PreStart()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            NinjectWebCommon.Start();
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
            var db = NinjectWebCommon.Kernel.Get<EntitiesContext>();
            
            if (db.Database.Exists()
                && db.Database.CompatibleWithModel(false) == false)
            {
                db.Database.Delete();
            }

            if (db.Database.CreateIfNotExists())
            {
                CreateDemoDatabaseEntries(db);
            }
        }

        private static async void CreateDemoDatabaseEntries(EntitiesContext db)
        {
            var demoUser = db.Users.Create();
            demoUser.UserName = "demo";

            using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
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