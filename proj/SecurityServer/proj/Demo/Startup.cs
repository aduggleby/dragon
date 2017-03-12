using Owin;

// [assembly: OwinStartup(typeof(Dragon.SecurityServer.Demo.Startup))] // The Startup class is set in appSettings.

namespace Dragon.SecurityServer.Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
