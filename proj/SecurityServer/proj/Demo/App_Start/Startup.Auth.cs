using System.Configuration;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
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
                    CookieName = "Dragon.SecurityServer.Demo",
                    AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                    AuthenticationMode = AuthenticationMode.Passive,
                });
            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    MetadataAddress = FederatedAuthentication.WSFederationAuthenticationModule.Issuer + "federationmetadata/2007-06/federationmetadata.xml",
                    Wtrealm = FederatedAuthentication.WSFederationAuthenticationModule.Realm,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new[] { ConfigurationManager.AppSettings["WtRealm"], ConfigurationManager.AppSettings["WtRealm"].ToLower() },
                        ValidIssuer = ConfigurationManager.AppSettings["ValidIssuer"],
                    }
                });
        }
    }
}