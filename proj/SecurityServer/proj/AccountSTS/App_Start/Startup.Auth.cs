using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Dragon.SecurityServer.AccountSTS.App_Start;
using Dragon.SecurityServer.AccountSTS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Owin;
using SimpleInjector;

namespace Dragon.SecurityServer.AccountSTS
{
    public partial class Startup
    {
        public static IDataProtectionProvider DataProtectionProvider { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app, Container container)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();

            // Configure the db context, user manager and signin manager to use a single instance per request
//            app.CreatePerOwinContext(ApplicationDbContext.Create);
//            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
//            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext(container.GetInstance<ApplicationUserManager>);

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
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),
                        OnException = (context =>
                        {
                            throw context.Exception;
                        })
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            EnableAuthenticationProviders(app);
        }

        private static void EnableAuthenticationProviders(IAppBuilder app)
        {
            var enabledProviders =
                WebConfigurationManager.AppSettings["AuthenticationProviders"].Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (IsEnabled("Microsoft", enabledProviders))
            {
                app.UseMicrosoftAccountAuthentication(
                    clientId: GetClientID("Microsoft"),
                    clientSecret: GetClientSecret("Microsoft"));
            }
            if (IsEnabled("Twitter", enabledProviders))
            {
                app.UseTwitterAuthentication(
                    consumerKey: GetClientID("Twitter"),
                    consumerSecret: GetClientSecret("Twitter"));
            }
            if (IsEnabled("Facebook", enabledProviders))
            {
                app.UseFacebookAuthentication(
                    appId: GetClientID("Facebook"),
                    appSecret: GetClientSecret("Facebook"));
            }
            if (IsEnabled("Google", enabledProviders))
            {
                var options = new GoogleOAuth2AuthenticationOptions
                {
                    ClientId = GetClientID("Google"),
                    ClientSecret = GetClientSecret("Google"),
                    
                    Provider = new GoogleOAuth2AuthenticationProvider
                    {
                        OnAuthenticated = context =>
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim("Google_AccessToken", context.AccessToken));
                            /*
                            // TODO: remove, see OnApplyRedirect below...
                            var token = context.TokenResponse.Value<string>("id_token");
                            var jwt = new JwtSecurityToken(token);
                            var identifier = jwt.Claims.FirstOrDefault(
                                    claim => string.Equals(claim.Type, "openid_id", StringComparison.InvariantCulture));
                            */

                            return Task.FromResult(0);
                        },
                        OnApplyRedirect = context =>
                        {
                            var dictionary = new Dictionary<string, string>
                            {
                                { "openid.realm", new Uri("http://localhost:51385/").GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped)},
                            };
                            var redirect = context.RedirectUri;
                            //var redirect = context.RedirectUri.Replace("response_type=code", "response_type=code id_token"); // TODO: this causes invalid an invalid return url that contains an anchor
                            var redirectUri = WebUtilities.AddQueryString(redirect, dictionary);
                            context.Response.Redirect(redirectUri);
                        },
                    },
                    BackchannelHttpHandler = new CustomWebRequestHandler()
                };
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
                app.UseGoogleAuthentication(options);
            }
        }

        private static string GetClientSecret(string providerName)
        {
            return WebConfigurationManager.AppSettings["AuthenticationProvider." + providerName + ".ClientSecret"];
        }

        private static string GetClientID(string providerName)
        {
            return WebConfigurationManager.AppSettings["AuthenticationProvider." + providerName + ".ClientID"];
        }

        private static bool IsEnabled(string providerName, IEnumerable<string> enabledProviders)
        {
            var isEnabled = enabledProviders.Contains(providerName);
            if (isEnabled)
            {
                if (string.IsNullOrWhiteSpace(GetClientID(providerName)) ||
                    string.IsNullOrWhiteSpace(GetClientSecret(providerName)))
                {
                    throw new ConfigurationErrorsException("Configuration for AuthenticationProvider " + providerName + " not found!");
                }
            }
            return isEnabled;
        }
    }
}