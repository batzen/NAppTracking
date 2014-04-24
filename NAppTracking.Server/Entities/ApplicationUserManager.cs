namespace NAppTracking.Server.Entities
{
    using Microsoft.AspNet.Identity;

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            // Configure the application user manager
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                //RequireUniqueEmail = true,
            };

            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 10,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
        }
    }
}