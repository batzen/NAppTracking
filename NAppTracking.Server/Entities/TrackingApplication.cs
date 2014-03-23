namespace NAppTracking.Server.Entities
{
    using System;
    using System.Collections.Generic;

    public class TrackingApplication
    {
        public TrackingApplication()
        {
            this.ApiKey = Guid.NewGuid().ToString().ToLowerInvariant();
            this.Owners = new HashSet<ApplicationUser>();
            this.ExceptionReports = new HashSet<ExceptionReport>();
        }

        public int Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ApiKey { get; set; }

        public ICollection<ApplicationUser> Owners { get; set; }

        public ICollection<ExceptionReport> ExceptionReports { get; set; }
    }
}