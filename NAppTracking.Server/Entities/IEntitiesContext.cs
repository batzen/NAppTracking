namespace NAppTracking.Server.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IEntitiesContext : IDisposable
    {
        DbSet<TrackingApplication> TrackingApplications { get; set; }

        DbSet<ExceptionReport> ExceptionReports { get; set; }

        DbSet<ExceptionReportFile> ExceptionReportFiles { get; set; }

        IDbSet<ApplicationUser> Users { get; set; }

        IDbSet<IdentityRole> Roles { get; set; }

        Database Database { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsync();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbSet Set(Type entityType);

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}