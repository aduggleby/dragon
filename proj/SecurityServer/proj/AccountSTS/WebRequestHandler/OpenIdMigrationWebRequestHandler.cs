using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json.Linq;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.WebRequestHandler
{
    /// <summary>
    /// Migrates Google logins from OpenID 2.0 to OpenID Connect.
    /// Based on <see href="http://stackoverflow.com/questions/26730568/google-openid-connect-migration-getting-the-openid-id-in-asp-net-app">this</see>.
    /// </summary>
    public class OpenIdMigrationWebRequestHandler : System.Net.Http.WebRequestHandler
    {
        private const string OpenIdConfigurationUri = "https://accounts.google.com/.well-known/openid-configuration";
        private const string NameIdentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string EmailAddressClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        private const string OpenId2IdClaimType = "openid_id";
        private const string LoginProviderName = "Google";
        private const string OpenId2LoginProviderName = "Google-OpenId2";
        private const string OpenId2ProviderKeyPrefix = "https://www.google.com/accounts/";
        private static readonly Uri GooglePlusMeUri = new Uri("https://www.googleapis.com/plus/v1/people/me");
        private static readonly JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDragonUserStore<AppMember> _userStore;

        public OpenIdMigrationWebRequestHandler(IDragonUserStore<AppMember> userStore)
        {
            _userStore = userStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponse = await base.SendAsync(request, cancellationToken);
            if (request.RequestUri == GooglePlusMeUri) return httpResponse;

            var configuration = await GetConfiguration(cancellationToken);
            var idToken = GetIdToken(httpResponse);
            var claims = GetClaims(idToken, configuration);

            try
            {
                await MigrateOpenId2ToOpenIdConnect(claims, idToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                StackExchange.Exceptional.ErrorStore.LogException(ex, null);
            }
            return httpResponse;
        }

        private static async Task<OpenIdConnectConfiguration> GetConfiguration(CancellationToken cancellationToken)
        {
            return await OpenIdConnectConfigurationRetriever.GetAsync(OpenIdConfigurationUri, cancellationToken); // TODO: cache
        }

        private static string GetIdToken(HttpResponseMessage httpResponse)
        {
            var response = JObject.Parse(httpResponse.Content.ReadAsStringAsync().Result);
            var idToken = response.Value<string>("id_token");
            return idToken;
        }

        private static ClaimsPrincipal GetClaims(string idToken, OpenIdConnectConfiguration configuration)
        {
            SecurityToken token;
            var claims = TokenHandler.ValidateToken(idToken, new TokenValidationParameters
            {
                ValidAudience = WebConfigurationManager.AppSettings["AuthenticationProvider.Google.OpenId2.ValidAudience"],
                ValidIssuer = WebConfigurationManager.AppSettings["AuthenticationProvider.Google.OpenId2.ValidIssuer"],
                IssuerSigningTokens = configuration.SigningTokens
            }, out token);
            return claims;
        }

        private async Task MigrateOpenId2ToOpenIdConnect(ClaimsPrincipal claims, string idToken)
        {
            var emailClaim = claims.FindFirst(EmailAddressClaimType);
            if (emailClaim == null)
            {
                throw new OpenIdMigrationException("Email not found for id_token: " + idToken);
            }
            var user = await _userStore.FindByEmailAsync(emailClaim.Value);
            if (user == null)
            {
                throw new OpenIdMigrationException("User not found: " + emailClaim.Value);
            }
            var openId2IdClaim = claims.FindFirst(OpenId2IdClaimType);
            if (openId2IdClaim == null)
            {
                throw new OpenIdMigrationException("OpenID 2.0 ID not found: " + emailClaim.Value);
            }
            var idClaim = claims.FindFirst(NameIdentifierClaimType);
            if (idClaim == null)
            {
                throw new OpenIdMigrationException("OpenID Connect Id not found: " + emailClaim.Value);
            }
            var logins = await _userStore.GetLoginsAsync(user);
            if (logins.Any(x => x.LoginProvider == LoginProviderName && x.ProviderKey.StartsWith(OpenId2ProviderKeyPrefix)))
            {
                Logger.Trace("OpenID 2.0 login found: " + user.Email);
                var obsoleteLogin = logins.FirstOrDefault(x => x.LoginProvider == LoginProviderName && x.ProviderKey == openId2IdClaim.Value);
                if (obsoleteLogin != null)
                {
                    await _userStore.AddLoginAsync(user, new UserLoginInfo(LoginProviderName, idClaim.Value));
                    await _userStore.AddLoginAsync(user, new UserLoginInfo(OpenId2LoginProviderName, openId2IdClaim.Value));
                    await _userStore.RemoveLoginAsync(user, obsoleteLogin);
                    Logger.Info("OpenID 2.0 to OpenID Connect: {0} => {1}.", openId2IdClaim.Value, idClaim.Value);
                }
                else
                {
                    Logger.Error($"Google OpenID 2.0 login could not be resolved: {user.Email}");
                    throw new Exception($"Unable to migrate from OpenID 2.0 to OpenID Connect: {user.Email} ({user.Id})");
                }
            }
        }
    }
}