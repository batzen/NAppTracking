namespace NAppTracking.Server.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TrackingApplication : IEntity
    {
        public TrackingApplication()
        {
            this.ApiKey = Guid.NewGuid();
            this.Owners = new HashSet<ApplicationUser>();
            this.ExceptionReports = new HashSet<ExceptionReport>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid ApiKey { get; set; }

        public virtual ICollection<ApplicationUser> Owners { get; set; }

        public virtual ICollection<ExceptionReport> ExceptionReports { get; set; }
    }
}