namespace NAppTracking.Server
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Helpers;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using NAppTracking.Server.Configuration;
    using NAppTracking.Server.Entities;
    using Ninject;

    public class AppActivator
    {
        public static void PreStart()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            System.Web.Mvc.ModelBinders.Binders.DefaultBinder = new EntityModelBinder();

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
            var db = NinjectWebCommon.Kernel.Get<IEntitiesContext>();
            
            if (db.Database.Exists()
                && db.Database.CompatibleWithModel(false) == false)
            {
                db.Database.Delete();
            }

            var haveToCreateDemoDatabaseEntries = NinjectWebCommon.Kernel.Get<IAppConfiguration>().IsDemoEnabled
                                                  && (db.Database.CreateIfNotExists() || db.Users.Any() == false);

            if (haveToCreateDemoDatabaseEntries)
            {
                CreateDemoDatabaseEntries(db);
            }
        }

        private static async void CreateDemoDatabaseEntries(IEntitiesContext db)
        {
            var demoUser = db.Users.Create();
            demoUser.UserName = "demo";

            using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>((DbContext)db)))
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