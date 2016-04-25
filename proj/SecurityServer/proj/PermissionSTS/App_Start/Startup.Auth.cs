using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.PermissionSTS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using SimpleInjector;

namespace Dragon.SecurityServer.PermissionSTS
{
    public partial class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app, Container container)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();

            // Configure the db context, user manager and signin manager to use a single instance per request
//            app.CreatePerOwinContext(ApplicationDbContext.Create);
           // app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AppMember>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    AuthenticationType = "securityserver",
                    Wreply = ConfigurationManager.AppSettings["SecurityTokenServiceEndpointUrl"],
                    Wtrealm = ConfigurationManager.AppSettings["WtRealm"],
                    MetadataAddress = ConfigurationManager.AppSettings["WsFederationEndpointUrl"] + "/FederationMetadata/2007-06/FederationMetadata.xml",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new[] { ConfigurationManager.AppSettings["WtRealm"], ConfigurationManager.AppSettings["WtRealm"].ToLower() },
                        ValidIssuer = ConfigurationManager.AppSettings["ValidIssuer"]
                    },
                    Notifications = new WsFederationAuthenticationNotifications()
                    {
                        RedirectToIdentityProvider = (ctx) =>
                        {
                            //To avoid a redirect loop to the federation server send 403 when user is authenticated but does not have access
                            if (ctx.OwinContext.Response.StatusCode == 401 && ctx.OwinContext.Authentication.User.Identity.IsAuthenticated)
                            {
                                ctx.OwinContext.Response.StatusCode = 403;
                                ctx.HandleResponse();
                            }
                            // forward parameters from the client or previous STS'e
                            var parameters = new[]{"action", "data", "serviceid", "signature", "expiry", "userid", "appid"};
                            foreach (var parameter in parameters)
                            {
                                ctx.ProtocolMessage.SetParameter(parameter, ctx.Request.Query[parameter]);
                            }
                            return Task.FromResult(0);
                        },
                        SecurityTokenValidated = (ctx) =>
                        {
                            var result = ctx.AuthenticationTicket;
                            if (result != null)
                            {
                                ctx.OwinContext.Authentication.SignOut("ExternalCookie");
                                var claims = result.Identity.Claims.ToList();
                                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, "External Account"));
                                var email = Enumerable.SingleOrDefault(claims, x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
                                var ci = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                                ctx.OwinContext.Authentication.SignIn(ci);
                            }
                            // var redirectUri = new Uri(ctx.AuthenticationTicket.Properties.RedirectUri, UriKind.RelativeOrAbsolute);
                            // var queryString = HttpUtility.ParseQueryString(redirectUri.Query);
                            // ctx.AuthenticationTicket.Properties.RedirectUri = queryString["wreply"];
                            return Task.FromResult(0);
                        }
                    }
                }
            );
            app.UseStageMarker(PipelineStage.Authenticate);

            /*
            //Remap logout to a federated logout
            app.Map(LogoutUrl, map =>
            {
                map.Run(ctx =>
                {
                    ctx.Authentication.SignOut();
                    return Task.FromResult(0);
                });
            });

            //Tell antiforgery to use the name claim
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
             */

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

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}