namespace NAppTracking.Server
{
    using System;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using NAppTracking.Server.Entities;
    using Owin;

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        // Only redirect when the request is a non API request
                        if (!ctx.Request.Path.StartsWithSegments(new PathString("/api")))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    },
                    //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                    //validateInterval: TimeSpan.FromSeconds(30),
                    //regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                    OnValidateIdentity = OnValidateIdentity

                }
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            app.UseGoogleAuthentication();
        }

        private static async Task OnValidateIdentity(CookieValidateIdentityContext context)
        {
            var validateInterval = TimeSpan.FromSeconds(30);

            var currentUtc = DateTimeOffset.UtcNow;
            if (context.Options != null 
                && context.Options.SystemClock != null)
            {
                currentUtc = context.Options.SystemClock.UtcNow;
            }

            var issuedUtc = context.Properties.IssuedUtc;
            var validate = !issuedUtc.HasValue;

            if (issuedUtc.HasValue)
            {
                validate = currentUtc.Subtract(issuedUtc.Value) > validateInterval;
            }

            if (!validate)
            {
                return;
            }

            var manager = DependencyResolver.Current.GetService<ApplicationUserManager>();
            var userId = context.Identity.GetUserId();

            if (manager != null
                && userId != null)
            {
                var user = await manager.FindByIdAsync(userId).ConfigureAwait(false);
                var reject = true;

                if (user != null
                    && manager.SupportsUserSecurityStamp)
                {
                    var securityStamp = context.Identity.FindFirstValue("AspNet.Identity.SecurityStamp");
                    if (securityStamp == await manager.GetSecurityStampAsync(userId).ConfigureAwait(false))
                    {
                        reject = false;
                        var identity = await user.GenerateUserIdentityAsync(manager);

                        if (identity != null)
                        {
                            context.OwinContext.Authentication.SignIn(new[] {identity});
                        }
                    }
                }

                if (reject)
                {
                    context.RejectIdentity();
                    context.OwinContext.Authentication.SignOut(new[] {context.Options.AuthenticationType});
                }
            }
        }
    }
}