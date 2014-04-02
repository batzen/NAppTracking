namespace NAppTracking.Server.Entities
{
    using System;
    using System.Collections.Generic;

    public class TrackingApplication : IEntity
    {
        public TrackingApplication()
        {
            this.ApiKey = Guid.NewGuid();

            if (this.Owners == null) this.Owners = new HashSet<ApplicationUser>();

            if (this.ExceptionReports == null) this.ExceptionReports = new HashSet<ExceptionReport>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid ApiKey { get; set; }

        public virtual ICollection<ApplicationUser> Owners { get; set; }

        public virtual ICollection<ExceptionReport> ExceptionReports { get; set; }
    }
}