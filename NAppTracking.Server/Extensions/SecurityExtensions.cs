namespace NAppTracking.Server
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using NAppTracking.Server.Entities;

    public static class SecurityExtensions
    {
        public static bool IsOwner(this TrackingApplication secureObject, IPrincipal user)
        {
            if (secureObject == null)
            {
                throw new ArgumentNullException("secureObject");
            }

            if (user == null || user.Identity == null)
            {
                return false;
            }

            return user.IsAdministrator() || secureObject.Owners.Any(u => u.UserName == user.Identity.Name);
        }

        public static bool IsOwner(this TrackingApplication secureObject, ApplicationUser user)
        {
            if (secureObject == null)
            {
                throw new ArgumentNullException("secureObject");
            }

            if (user == null)
            {
                return false;
            }

            return secureObject.Owners.Any(u => u.Id == user.Id);
        }

        public static bool IsOwner(this ExceptionReport secureObject, IPrincipal user)
        {
            if (secureObject == null)
            {
                throw new ArgumentNullException("secureObject");
            }

            if (secureObject.Application == null)
            {
                return false;
            }

            return secureObject.Application.IsOwner(user);
        }

        public static bool IsOwner(this ExceptionReport secureObject, ApplicationUser user)
        {
            if (secureObject == null)
            {
                throw new ArgumentNullException("secureObject");
            }

            if (secureObject.Application == null)
            {
                return false;
            }

            return secureObject.Application.IsOwner(user);
        }
    }
}