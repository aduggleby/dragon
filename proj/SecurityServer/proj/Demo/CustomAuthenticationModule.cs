using System.IdentityModel.Services;
using System.Web;
using System.Web.Configuration;

namespace Dragon.SecurityServer.Demo
{
    // Handles returning to the originally called URL, not the statically predefined one.
    // Registering to the RedirectingToIdentityProvider event did not seem to work.
    public class CustomAuthenticationModule : WSFederationAuthenticationModule
    {
        protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
        {
            e.SignInRequestMessage.Reply = HttpContext.Current.Request.Url.ToString();
            e.SignInRequestMessage.Parameters.Add("serviceid", WebConfigurationManager.AppSettings["ServiceId"]);
            base.OnRedirectingToIdentityProvider(e);
        }
    }
}