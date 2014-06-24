namespace NAppTracking.Server.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using NAppTracking.Server.Entities;

    public class DemoDatabaseHelper
    {
        public static void CreateDemoDatabase()
        {
            var db = DependencyResolver.Current.GetService<EntitiesContext>();

            DropTablesIfModelIsNotCompatible(db);

            var haveToCreateDemoDatabaseEntries = db.Database.CreateIfNotExists() || db.Users.Any() == false;

            if (haveToCreateDemoDatabaseEntries)
            {
                CreateDemoDatabaseEntries(db);
            }
        }

        private static void DropTablesIfModelIsNotCompatible(DbContext db)
        {
            if (db.Database.Exists() == false 
                || db.Database.CompatibleWithModel(false))
            {
                return;
            }

            if (db.Database.Connection.State != ConnectionState.Open)
            {
                db.Database.Connection.Open();
            }

            var tables = GetTables(db);

            DropTables(db, tables);
        }

        private static void DropTables(DbContext db, ICollection<string> tables)
        {
            // Just drop all tables multiple times. This way we can delete all tables regardless of PKs and FKs.
            for (var i = 0; i < 5; i++)
            {
                var removedTables = new List<string>();

                foreach (var table in tables)
                {
                    try
                    {
                        using (var command = db.Database.Connection.CreateCommand())
                        {
                            command.CommandText = string.Format("DROP TABLE {0}", table);
                            command.ExecuteNonQuery();
                            removedTables.Add(table);
                        }
                    }
                    catch
                    {
                    }
                }

                foreach (var removedTable in removedTables)
                {
                    tables.Remove(removedTable);
                }
            }
        }

        private static List<string> GetTables(DbContext db)
        {
            var tables = new List<string>();
            using (var command = db.Database.Connection.CreateCommand())
            {
                command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }
            return tables;
        }

        private static async void CreateDemoDatabaseEntries(EntitiesContext db)
        {
            var demoUser = db.Users.Create();
            demoUser.UserName = "demo";
            demoUser.Email = "demouser@napptracking.net";
            demoUser.EmailConfirmed = true;

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