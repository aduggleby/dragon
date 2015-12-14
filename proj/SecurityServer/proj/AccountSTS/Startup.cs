using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Dragon.SecurityServer.AccountSTS.Startup))]
namespace Dragon.SecurityServer.AccountSTS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = SimpleInjectorInitializer.Initialize(app);
            ConfigureAuth(app, container);
        }
    }
}
