namespace NAppTracking.Server
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using NAppTracking.Server.Configuration;
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

            ConfigureThirdPartyLoginProviders(app);
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

        private static void ConfigureThirdPartyLoginProviders(IAppBuilder app)
        {
            var configurationService = DependencyResolver.Current.GetService<ConfigurationService>();

            ConfigureFacebookLoginProvider(app, configurationService);

            ConfigureGoogleLoginProvider(app, configurationService);

            ConfigureMicrosoftLoginProvider(app, configurationService);

            ConfigureTwitterLoginProvider(app, configurationService);
        }

        private static void ConfigureFacebookLoginProvider(IAppBuilder app, ConfigurationService configurationService)
        {
            if (Convert.ToBoolean(configurationService.ReadSetting("Auth.Facebook.Enabled")))
            {
                app.UseFacebookAuthentication(appId: configurationService.ReadSetting("Auth.Facebook.Id"), appSecret: configurationService.ReadSetting("Auth.Facebook.Id"));
            }
        }

        private static void ConfigureGoogleLoginProvider(IAppBuilder app, ConfigurationService configurationService)
        {
            if (Convert.ToBoolean(configurationService.ReadSetting("Auth.Google.Enabled")))
            {
                app.UseGoogleAuthentication();
            }
        }

        private static void ConfigureMicrosoftLoginProvider(IAppBuilder app, ConfigurationService configurationService)
        {
            if (Convert.ToBoolean(configurationService.ReadSetting("Auth.Microsoft.Enabled")))
            {
                app.UseMicrosoftAccountAuthentication(clientId: configurationService.ReadSetting("Auth.Microsoft.Id"), clientSecret: configurationService.ReadSetting("Auth.Microsoft.Secret"));
            }
        }

        private static void ConfigureTwitterLoginProvider(IAppBuilder app, ConfigurationService configurationService)
        {
            if (Convert.ToBoolean(configurationService.ReadSetting("Auth.Twitter.Enabled")))
            {
                app.UseTwitterAuthentication(consumerKey: configurationService.ReadSetting("Auth.Twitter.Id"), consumerSecret: configurationService.ReadSetting("Auth.Twitter.Id"));
            }
        }
    }
}