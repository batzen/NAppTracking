using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NAppTracking.Server.Startup))]
namespace NAppTracking.Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
