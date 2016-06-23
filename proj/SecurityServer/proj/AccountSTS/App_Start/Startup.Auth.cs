using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.AccountSTS.WebRequestHandler;
using Facebook;
using LinqToTwitter;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.Twitter;
using Owin;
using SimpleInjector;

namespace Dragon.SecurityServer.AccountSTS
{
    public partial class Startup
    {
        public static IDataProtectionProvider DataProtectionProvider { get; private set; }
        public static OpenIdMigrationWebRequestHandler OpenIdMigrationWebrequestHandler { get; set; }

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
                var options = new MicrosoftAccountAuthenticationOptions
                {
                    ClientId = GetClientID("Microsoft"),
                    ClientSecret = GetClientSecret("Microsoft"),
                    Provider = new MicrosoftAccountAuthenticationProvider
                    {
                        OnAuthenticated = context =>
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:microsoftaccount:access_token", context.AccessToken));
                            foreach (var claim in context.User)
                            {
                                var claimType = string.Format("urn:microsoftaccount:{0}", claim.Key);
                                var claimValue = claim.Value.ToString();
                                if (!context.Identity.HasClaim(claimType, claimValue))
                                    context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Microsoft"));
                            }
                            return Task.FromResult(0);
                        }
                    }
                };
                options.Scope.Add("wl.emails");
                options.Scope.Add("wl.basic");
                app.UseMicrosoftAccountAuthentication(options);
            }
            if (IsEnabled("Twitter", enabledProviders))
            {
                var options = new TwitterAuthenticationOptions
                {
                    ConsumerKey = GetClientID("Twitter"),
                    ConsumerSecret = GetClientSecret("Twitter"),
                    Provider = new TwitterAuthenticationProvider
                    {
                        OnAuthenticated = (context) =>
                        {
                            var authTwitter = new SingleUserAuthorizer
                            {
                                CredentialStore = new SingleUserInMemoryCredentialStore
                                {
                                    ConsumerKey = GetClientID("Twitter"),
                                    ConsumerSecret = GetClientSecret("Twitter"),
                                    OAuthToken = context.AccessToken,
                                    OAuthTokenSecret = context.AccessTokenSecret,
                                    UserID = ulong.Parse(context.UserId),
                                    ScreenName = context.ScreenName
                                }
                            };
                            var twitterCtx = new TwitterContext(authTwitter);
                            var verifyResponse = (
                                from acct
                                in twitterCtx.Account
                                where (acct.Type == AccountType.VerifyCredentials) && acct.IncludeEmail
                                select acct
                                ).SingleOrDefault();
                            if (verifyResponse?.User != null)
                            {
                                var twitterUser = verifyResponse.User;
                                context.Identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Email, twitterUser.Email));
                            }
                            return Task.FromResult(0);
                        }
                    }
                };
                app.UseTwitterAuthentication(options);
            }
            if (IsEnabled("Facebook", enabledProviders))
            {
                var facebookOptions = new FacebookAuthenticationOptions
                {
                    AppId = GetClientID("Facebook"),
                    AppSecret = GetClientSecret("Facebook"),
                    Provider = new FacebookAuthenticationProvider
                    {
                        OnAuthenticated = (context) =>
                        {
                            var client = new FacebookClient(context.AccessToken);
                            dynamic info = client.Get("me", new { fields = "name,id,email" });
                            context.Identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Email, info.email));
                            return Task.FromResult(0);
                        }
                    }
                };
                app.UseFacebookAuthentication(facebookOptions);
            }
            if (IsEnabled("Google", enabledProviders))
            {
                var options = new GoogleOAuth2AuthenticationOptions
                {
                    ClientId = GetClientID("Google"),
                    ClientSecret = GetClientSecret("Google"),
                    Provider = new GoogleOAuth2AuthenticationProvider
                    {
                        OnApplyRedirect = context =>
                        {
                            var dictionary = new Dictionary<string, string>
                            {
                                { "openid.realm", new Uri(WebConfigurationManager.AppSettings["AuthenticationProvider.Google.OpenId2.Realm"]).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped)},
                            };
                            context.Response.Redirect(WebUtilities.AddQueryString(context.RedirectUri, dictionary));
                        },
                    },
                    BackchannelHttpHandler = OpenIdMigrationWebrequestHandler
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