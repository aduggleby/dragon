using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.Common;
using WebGrease.Css.Extensions;

namespace Dragon.SecurityServer.Demo
{
    // Handles returning to the originally called URL, not the statically predefined one.
    // Registering to the RedirectingToIdentityProvider event did not seem to work.
    public class CustomAuthenticationModule : WSFederationAuthenticationModule
    {
        protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
        {
            var hmacHelper = new HmacHelper
            {
                HmacService = new HmacSha256Service(),
            };
            var hmacSettings = HmacHelper.ReadHmacSettings();

            e.SignInRequestMessage.Reply = HttpContext.Current.Request.Url.ToString();
            var parameters = new Dictionary<string, string>
            {
                { "expiry", DateTime.UtcNow.AddMinutes(+15).Ticks.ToString() },
                { "serviceid", hmacSettings.ServiceId },
                { "appid", hmacSettings.AppId },
                { "userid", hmacSettings.UserId },
            };
            parameters.Add("signature", hmacHelper.CalculateHash(parameters, hmacSettings.Secret));
            parameters.ForEach(e.SignInRequestMessage.Parameters.Add);
            base.OnRedirectingToIdentityProvider(e);
        }
    }
}