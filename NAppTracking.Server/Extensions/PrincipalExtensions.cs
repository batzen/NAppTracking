namespace NAppTracking.Server
{
    using System.Security.Principal;

    public static class PrincipalExtensions
    {
        public static bool IsAdministrator(this IPrincipal self)
        {
            return self.IsInRole(Constants.AdminRoleName);
        }
    }
}