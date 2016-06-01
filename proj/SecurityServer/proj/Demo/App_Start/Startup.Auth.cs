using System.IdentityModel.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;

namespace Dragon.SecurityServer.Demo
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ExternalCookie);

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                    AuthenticationMode = AuthenticationMode.Passive,
                });
            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    MetadataAddress = FederatedAuthentication.WSFederationAuthenticationModule.Issuer + "federationmetadata/2007-06/federationmetadata.xml",
                    Wtrealm = FederatedAuthentication.WSFederationAuthenticationModule.Realm,
                });
        }
    }
}