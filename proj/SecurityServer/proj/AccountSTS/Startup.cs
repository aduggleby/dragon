using Microsoft.Owin;
using Owin;
using static Dragon.SecurityServer.AccountSTS.SimpleInjectorInitializer;

[assembly: OwinStartup(typeof(Dragon.SecurityServer.AccountSTS.Startup))]
namespace Dragon.SecurityServer.AccountSTS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = Initialize(app);
            ConfigureAuth(app, container);
        }
    }
}
