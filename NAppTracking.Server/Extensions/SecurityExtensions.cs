namespace NAppTracking.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using Microsoft.AspNet.Identity;
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

            var userId = user.Identity.GetUserId();
            return user.IsAdministrator() || secureObject.Owners.Any(u => u.Id == userId);
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

        public static IQueryable<TSource> OwnedBy<TSource>(this IQueryable<TSource> source, IPrincipal user)
            where TSource : class, IHasOwners
        {
            return source.OwnedBy(user.Identity);
        }

        public static IQueryable<TSource> OwnedBy<TSource>(this IQueryable<TSource> source, IIdentity user)
            where TSource : class, IHasOwners
        {
            var userId = user.GetUserId();
            return source.Where(item => item.Owners.Any(owner => owner.Id == userId));
        }

        public static IQueryable<TSource> ApplicationOwnedBy<TSource>(this IQueryable<TSource> source, IPrincipal user)
            where TSource : ExceptionReport
        {
            return source.ApplicationOwnedBy(user.Identity);
        }

        public static IQueryable<TSource> ApplicationOwnedBy<TSource>(this IQueryable<TSource> source, IIdentity user)
            where TSource : ExceptionReport
        {
            var userId = user.GetUserId();
            return source.Where(item => item.Application.Owners.Any(owner => owner.Id == userId));
        }
    }

    public interface IHasOwners
    {
        ICollection<ApplicationUser> Owners { get; set; }
    }
}