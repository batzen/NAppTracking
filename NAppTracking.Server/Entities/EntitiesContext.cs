namespace NAppTracking.Server.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class EntitiesContext : IdentityDbContext<ApplicationUser>
    {
        public EntitiesContext()
            : base("DefaultConnection", false)
        {
        }

        public DbSet<TrackingApplication> TrackingApplications { get; set; }

        public DbSet<ExceptionReport> ExceptionReports { get; set; }

        public DbSet<ExceptionReportFile> ExceptionReportFiles { get; set; }

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
            modelBuilder.Entity<ExceptionReport>()
                .Property(x => x.CreatedUtc)
                .IsRequired();
            modelBuilder.Entity<ExceptionReport>()
                .HasMany(x => x.ExceptionReportFiles)
                .WithRequired(x => x.ExceptionReport);

            modelBuilder.Entity<ExceptionReportFile>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<ExceptionReportFile>()
                .Property(x => x.CreatedUtc)
                .IsRequired();
            modelBuilder.Entity<ExceptionReportFile>()
                .Property(x => x.StorageId)
                .IsRequired();
            modelBuilder.Entity<ExceptionReportFile>()
                .Property(x => x.DisplayName)
                .IsRequired();
            modelBuilder.Entity<ExceptionReportFile>()
                .Property(x => x.FileName)
                .IsRequired();
        }

        public override int SaveChanges()
        {
            this.EnsureUtcDatesInEntities();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.EnsureUtcDatesInEntities();

            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        private void EnsureUtcDatesInEntities()
        {
            var context = ((IObjectContextAdapter)this).ObjectContext;

            // Find all Entities that are Added/Modified, not a relationship and implement our interfaces
            var objectStateEntries = context.ObjectStateManager
                .GetObjectStateEntries(EntityState.Added | EntityState.Modified)
                .Where(e => e.IsRelationship == false && (e.Entity is ICreatedUtc || e.Entity is IModifiedUtc));

            var currentUtcTime = DateTime.UtcNow;

            foreach (var entry in objectStateEntries)
            {
                var createdUtc = entry.Entity as ICreatedUtc;

                if (createdUtc != null
                    && entry.State == EntityState.Added)
                {
                    createdUtc.CreatedUtc = currentUtcTime;
                }

                var modifiedUtc = entry.Entity as IModifiedUtc;

                if (modifiedUtc != null)
                {
                    modifiedUtc.ModifiedUtc = currentUtcTime;
                }
            }
        }
    }
}