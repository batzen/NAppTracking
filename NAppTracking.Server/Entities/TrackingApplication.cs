namespace NAppTracking.Server.Entities
{
    using System;
    using System.Collections.Generic;

    public class TrackingApplication : IEntity
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual Guid ApiKey { get; set; }

        public virtual ICollection<ApplicationUser> Owners { get; set; }

        public virtual ICollection<ExceptionReport> ExceptionReports { get; set; }
    }
}