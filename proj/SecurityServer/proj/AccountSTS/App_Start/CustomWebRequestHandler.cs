using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json.Linq;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.App_Start
{
    /// <summary>
    /// Found <see href="http://stackoverflow.com/questions/26730568/google-openid-connect-migration-getting-the-openid-id-in-asp-net-app">here</see>.
    /// </summary>
    public class CustomWebRequestHandler : WebRequestHandler
    {
        private static readonly Uri GooglePlusMeUri = new Uri("https://www.googleapis.com/plus/v1/people/me");
        private const string OpenIdConfigurationUri = "https://accounts.google.com/.well-known/openid-configuration";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponse = await base.SendAsync(request, cancellationToken);
            if (request.RequestUri == GooglePlusMeUri) return httpResponse;
            var configuration = await OpenIdConnectConfigurationRetriever.GetAsync(OpenIdConfigurationUri, cancellationToken); // TODO: cache
            var jwt = httpResponse.Content.ReadAsStringAsync().Result;
            var response = JObject.Parse(jwt);
            var idToken = response.Value<string>("id_token");
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken token;
                var claims = tokenHandler.ValidateToken(idToken, new TokenValidationParameters
                {
                    ValidAudience = WebConfigurationManager.AppSettings["AuthenticationProvider.Google.OpenId2.ValidAudience"],
                    ValidIssuer = WebConfigurationManager.AppSettings["AuthenticationProvider.Google.OpenId2.ValidIssuer"],
                    IssuerSigningTokens = configuration.SigningTokens
                }, out token);

                var claim = claims.FindFirst("openid_id");
                if (claim == null)
                {
                    Logger.Warn("Openid_id not found for id_token: " + idToken);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return httpResponse;
        }
    }
}