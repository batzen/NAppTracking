namespace NAppTracking.Server.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TrackingApplication
    {
        public int Key { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ApiKey { get; set; }

        public ICollection<ApplicationUser> Owners { get; set; }

        public ICollection<ExceptionReport> ExceptionReports { get; set; }
    }
}