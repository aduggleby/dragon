using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Common;
using Dragon.SecurityServer.AccountSTS.Helpers;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IDragonUserStore<AppMember> _userStore;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IDragonUserStore<AppMember> userStore)
        {
            _userStore = userStore;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            // keep the federation context
            var routeValues = new RouteValueDictionary();
            if (Request != null && Request.QueryString != null)
            {
                foreach (var key in Request.QueryString.AllKeys)
                {
                    routeValues.Add(key, Request.QueryString.Get(key));
                }
            }
            ViewBag.RouteValues = routeValues;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.RouteValues["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    Logger.Trace("Login failed: user {0} is locked out", model.Email);
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    Logger.Trace("Login failed: user {0} is unverified", model.Email);
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    if (!IsUserLoggedIn())
                    {
                        Logger.Trace("Login failed: user {0}", model.Email);
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                    else
                    {
                        HandleUserNotRegisteredForService();
                        Logger.Trace("Login failed: user {0} not registered for service {1}", model.Email,
                            RequestHelper.GetCurrentServiceId());
                        ModelState.AddModelError("", await GenerateLoginWithServiceErrorMessage(model));
                    }
                    return View(model);
            }
        }

        private async Task<string> GenerateLoginWithServiceErrorMessage(LoginViewModel model)
        {
            var message = "Invalid login attempt.";
            var user = await _userStore.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var availableLogins = await _userStore.GetLoginsAsync(user);
                if (availableLogins.Any())
                {
                    message += string.Format(" Try logging in using {0}{1}.", availableLogins.Count > 1 ? "one of " : "",
                        availableLogins.Select(x => x.LoginProvider).Aggregate((x1, x2) => string.Format("{0}, {1}", x1, x2)));
                }
            }
            return message;
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    Logger.Trace("Verify code: locked out (code {0})", model.Code);
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    Logger.Trace("Verify code: invalid code {0}", model.Code);
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppMember { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userStore.AddServiceToUserAsync(user, RequestHelper.GetCurrentServiceId()); // throws if already registered, but it's a new user

                    //  Comment the following line to prevent log in until the user is confirmed.
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    var returnUrl = Request.QueryString["ReturnUrl"];
                    return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Home") : RedirectToLocal(returnUrl);
                }
                Logger.Trace("Register failed: {0}", string.Join("; ", result.Errors));

                // avoid Name x is already taken. errors
                var identityResult = new IdentityResult(result.Errors.Where(x => !x.StartsWith("Name ")));
                AddErrors(identityResult);

                // Show a hint about external providers
                ModelState.AddModelError("",
                    "You might already have registered using " + WebConfigurationManager.AppSettings["AuthenticationProviders"].ReplaceLast(",", " or ") +
                    ". If this is the case, login with one of those to set a password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                Logger.Trace("Confirm email failed: userId ({0}) or code ({1}) are null", userId, code);
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (!result.Succeeded)
            {
                  Logger.Trace("Confirm email failed: userId {0}, code {1}", userId, code);
            }
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    Logger.Trace("Forgot password failed: user {0} not found", model.Email);
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            Logger.Trace("Forgot password failed: invalid model state");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Logger.Trace("Reset password failed: invalid model state");
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                Logger.Trace("Reset password failed: user not found");
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            Logger.Trace("Reset password failed: {0}", string.Join("; ", result.Errors));
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                Logger.Trace("Send code failed: user not found");
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Logger.Trace("Send code failed: model state invalid");
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                Logger.Trace("Send code failed: SendTwoFactorCodeAsync failed");
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                Logger.Trace("External login failed: external login info is null");
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    Logger.Trace("External login failed: user {0} is locked out", loginInfo.Email);
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    Logger.Trace("External login failed: user {0} requires verification", loginInfo.Email);
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    if (IsUserLoggedIn())
                    {
                        HandleUserNotRegisteredForService();
                        ViewBag.RouteValues.ReturnUrl = returnUrl;
                        ViewBag.RouteValues.RememberMe = false;
                        return RedirectToAction("Login", ViewBag.RouteValues);
                    }
                    Logger.Trace("External login: user {0} does not have an account", loginInfo.Email);
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.RouteValues["ReturnUrl"] = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.RouteValues["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                Logger.Trace("External login confirmation failed: invalid model state");
                return View(model);
            }

            // Get the information about the user from the external login provider
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                Logger.Trace("External login confirmation failed: invalid external login info");
                return View("ExternalLoginFailure");
            }
            var user = await _userStore.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // User exists, add external login to existing account

                ViewBag.RequireLogin = true;
                // Show Login Form
                if (model.Password == null)
                {
                    Logger.Trace("External login confirmation: account {0} exists, link external login {1}", user.Email, info.Login.LoginProvider);
                    ModelState.AddModelError("", string.Format("Please login to link your account with your {0} account.", info.Login.LoginProvider));
                    ViewBag.LoginProvider = info.Login.LoginProvider;
                    return View(model);
                }

                // Process Login
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("password", "The Password field is required.");
                }
                if (!ModelState.IsValid)
                {
                    Logger.Trace("External login confirmation: model state invalid");
                    return View(model);
                }
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        break;
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        ViewBag.RouteValues.ReturnUrl = returnUrl;
                        ViewBag.RouteValues.RememberMe = false;
                        return RedirectToAction("SendCode", ViewBag.RouteValues);
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        if (!IsUserLoggedIn())
                        {
                            Logger.Trace("External login confirmation failed: user {0}, status {1}", model.Email, result);
                        }
                        else
                        {
                            HandleUserNotRegisteredForService();
                            Logger.Trace("External login confirmation failed: user {0} is not registered for service {1}", model.Email, RequestHelper.GetCurrentServiceId());
                        }
                        return View(model);
                }
            }
            else
            {
                user = await CreateUser(model.Email, info);
                if (user == null) return View(model);
            }
            if (!(await _userStore.GetLoginsAsync(user)).Any(x =>
                            info.Login.LoginProvider == x.LoginProvider &&
                            info.Login.ProviderKey == x.ProviderKey))
            {
                await _userStore.AddLoginAsync(user, info.Login);
            }
            await AddServiceToUser(user, RequestHelper.GetCurrentServiceId());
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToLocal(returnUrl);
        }

        /// <summary>
        /// Logout user if not registered for current service, adds error.
        /// </summary>
        private void HandleUserNotRegisteredForService()
        {
            ModelState.AddModelError("", " Not registered for this service!");
            // On service id mismatch user is logged in, but should not
            // Using ApplicationCookie because of https://aspnetidentity.codeplex.com/workitem/2347
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        private bool IsUserLoggedIn()
        {
            return AuthenticationManager.User != null && AuthenticationManager.User.Identity.IsAuthenticated;
        }

        private async Task AddServiceToUser(AppMember user, string serviceId)
        {
            if (await _userStore.IsUserRegisteredForServiceAsync(user, serviceId))
            {
                return;
            }
            var result = _userStore.AddServiceToUserAsync(user, serviceId);
            await result;
            if (result.IsFaulted)
            {
                Logger.Trace("Add service to user failed: user {0}, service {1}", user.Email, serviceId);
                ModelState.AddModelError("", "Unable to add service.");
            }
        }

        private async Task<AppMember> CreateUser(string email, ExternalLoginInfo info)
        {
            var user = new AppMember {UserName = email, Email = email};
            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (result.Succeeded)
                {
                    return await Task.FromResult(user);
                }
                else
                {
                    Logger.Trace("Create user failed: {0}", string.Join("; ", result.Errors));
                    AddErrors(result);
                }
            }
            else
            {
                Logger.Trace("Create user failed: {0}", string.Join("; ", result.Errors));
                AddErrors(result);
            }
            return await Task.FromResult<AppMember>(null);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            var returnUrl = Request.QueryString["returnUrl"] + WebConfigurationManager.AppSettings["SignOutPath"];
            return string.IsNullOrEmpty(returnUrl) ? (ActionResult) RedirectToAction("Index", "Home") : Redirect(returnUrl);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

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
        #endregion
    }
}