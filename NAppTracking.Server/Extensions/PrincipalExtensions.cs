namespace NAppTracking.Server
{
    using System.Security.Principal;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using NAppTracking.Server.Entities;

    public static class PrincipalExtensions
    {
        public static bool IsAdministrator(this IPrincipal self)
        {
            return self.IsInRole(Constants.AdminRoleName);
        }

        public static ApplicationUser AsApplicationUser(this IPrincipal user)
        {
            if (user == null
                || user.Identity == null)
            {
                return null;
            }

            var userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();

            return user.Identity.IsAuthenticated
                ? userManager.FindById(user.Identity.GetUserId()) 
                : null;
        }
    }
}