using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    public class FederationService : IFederationService
    {
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ActionResult PerformExternalLogin(HttpContextBase context, string provider, string returnUrl)
        {
            context.Session?.RemoveAll();
            return new ChallengeResult(provider, returnUrl);
        }

        public ActionResult PerformExternalLogin(HttpContextBase context, string provider, string returnUrl, string userId)
        {
            context.Session?.RemoveAll();
            return new ChallengeResult(provider, returnUrl);
        }

        public async Task<ActionResult> Disconnect(ApplicationSignInManager signInManager, ApplicationUserManager userManager, string provider, string userId, string redirectUri)
        {
            var userLoginInfo = (await userManager.GetLoginsAsync(userId)).First(x => x.LoginProvider == provider);

            var result = await userManager.RemoveLoginAsync(userId, userLoginInfo);
            if (result.Succeeded)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
            }
            else
            {
                Logger.Error($"Unable to disconnect {provider} from user {userId}");
            }
            return new RedirectResult(redirectUri);
        }

        /// <summary>
        ///  Redirects to the external login provider
        /// </summary>
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}