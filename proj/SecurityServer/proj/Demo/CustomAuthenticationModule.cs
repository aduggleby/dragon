using System.IdentityModel.Services;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.GenericSTSClient;
using WebGrease.Css.Extensions;

namespace Dragon.SecurityServer.Demo
{
    // Handles returning to the originally called URL, not the statically predefined one.
    // Registering to the RedirectingToIdentityProvider event did not seem to work.
    public class CustomAuthenticationModule : WSFederationAuthenticationModule
    {
        private static readonly HmacHelper HmacHelper = new HmacHelper { HmacService = new HmacSha256Service() };

        protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
        {
            var parameters = HmacHelper.CreateHmacRequestParametersFromConfig(Consts.ProfileHmacSettingsPrefix);
            parameters.ForEach(e.SignInRequestMessage.Parameters.Add);
            base.OnRedirectingToIdentityProvider(e);
        }
    }
}