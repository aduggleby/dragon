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
using Tweetinvi;

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
                CookieName = "Dragon.SecurityServer.AccountSTS",
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
                app.UseMicrosoftAccountAuthentication(GetMicrosoftAuthenticationOptions());
            }
            if (IsEnabled("Twitter", enabledProviders))
            {
                app.UseTwitterAuthentication(GetTwitterAuthenticationOptions());
            }
            if (IsEnabled("Facebook", enabledProviders))
            {
                app.UseFacebookAuthentication(GetFacebookAuthenticationOptions());
            }
            if (IsEnabled("Google", enabledProviders))
            {
                app.UseGoogleAuthentication(GetGoogleAuthenticationOptions());
            }
        }

        private static GoogleOAuth2AuthenticationOptions GetGoogleAuthenticationOptions()
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
                            {
                                "openid.realm",
                                new Uri(WebConfigurationManager.AppSettings["AuthenticationProvider.Google.OpenId2.Realm"]).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped)
                            },
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
            return options;
        }

        private static FacebookAuthenticationOptions GetFacebookAuthenticationOptions()
        {
            var facebookOptions = new FacebookAuthenticationOptions
            {
                AppId = GetClientID("Facebook"),
                AppSecret = GetClientSecret("Facebook"),
                Provider = new FacebookAuthenticationProvider
                {
                    OnAuthenticated = (context) =>
                    {
                        FacebookClient.DefaultVersion = "v2.6";
                        var client = new FacebookClient(context.AccessToken);
                        dynamic info = client.Get("me", new {fields = "name,id,email"});
                        if (!string.IsNullOrWhiteSpace(info.email))
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Email, info.email));
                        }
                        return Task.FromResult(0);
                    }
                }
            };
            facebookOptions.Scope.Add("email");
            return facebookOptions;
        }

        private static TwitterAuthenticationOptions GetTwitterAuthenticationOptions()
        {
            var options = new TwitterAuthenticationOptions
            {
                ConsumerKey = GetClientID("Twitter"),
                ConsumerSecret = GetClientSecret("Twitter"),
                Provider = new TwitterAuthenticationProvider
                {
                    OnAuthenticated = (context) =>
                    {
                        var twitterUser = Auth.ExecuteOperationWithCredentials(
                            Auth.CreateCredentials(GetClientID("Twitter"), GetClientSecret("Twitter"),
                                context.AccessToken, context.AccessTokenSecret),
                            () => User.GetAuthenticatedUser(parameters: new Tweetinvi.Core.Parameters.GetAuthenticatedUserParameters()
                            {
                                IncludeEmail = true
                            }));
                        if (!string.IsNullOrWhiteSpace(twitterUser?.Email))
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Email, twitterUser.Email));
                        }
                        return Task.FromResult(0);
                    }
                },
                // avoid invalid certificate errors, see http://stackoverflow.com/questions/36330675/get-users-email-from-twitter-api-for-external-login-authentication-asp-net-mvc
                BackchannelCertificateValidator = new Microsoft.Owin.Security.CertificateSubjectKeyIdentifierValidator(new[]
                {
                    "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                    "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                    "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                    "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                    "‎add53f6680fe66e383cbac3e60922e3b4c412bed", // Symantec Class 3 EV SSL CA - G3
                    "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
                    "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
                    "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
                }),
            };
            return options;
        }

        private static MicrosoftAccountAuthenticationOptions GetMicrosoftAuthenticationOptions()
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
            return options;
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