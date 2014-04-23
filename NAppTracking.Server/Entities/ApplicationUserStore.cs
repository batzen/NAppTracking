namespace NAppTracking.Server.Entities
{
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationUserStore : UserStore<ApplicationUser>
    {
        public ApplicationUserStore(EntitiesContext db)
            : base(db)
        {
        }
    }
}