namespace NAppTracking.Server.Entities
{
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity.EntityFramework;

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            if (this.Applications == null) this.Applications = new HashSet<TrackingApplication>();
        }

        public virtual ICollection<TrackingApplication> Applications { get; set; }
    }
}