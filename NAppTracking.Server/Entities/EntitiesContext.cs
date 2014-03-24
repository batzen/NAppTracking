namespace NAppTracking.Server.Entities
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class EntitiesContext : IdentityDbContext<ApplicationUser>
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx    
        public EntitiesContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<TrackingApplication> TrackingApplications { get; set; }

        public DbSet<ExceptionReport> ExceptionReports { get; set; }

        #region Overrides of DbContext

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.Applications);

            modelBuilder.Entity<TrackingApplication>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<TrackingApplication>()
                .HasMany<ExceptionReport>(x => x.ExceptionReports)
                .WithRequired(x => x.Application);

            modelBuilder.Entity<TrackingApplication>()
                .HasMany<ApplicationUser>(x => x.Owners)
                .WithMany(x => x.Applications)
                .Map(c => c.ToTable("TrackingApplicationOwners")
                    .MapLeftKey("ApplicationId")
                    .MapRightKey("UserId"));

            modelBuilder.Entity<ExceptionReport>()
                .HasKey(x => x.Id);
        }

        #endregion
    }
}