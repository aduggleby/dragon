using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Dragon.SecurityServer.AccountSTS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;

namespace Dragon.SecurityServer.AccountSTS.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IList<string> _adminUserIds;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            InitAdminUserIds();
            if (!_adminUserIds.Contains(HttpContext.User.Identity.GetUserId()))
            {
                Logger.Warn($"Unauthorized impersonate request from {HttpContext.User.Identity.GetUserId()}");
                throw new HttpException(401, "Unauthorized");
            }
        }

        private void InitAdminUserIds()
        {
            _adminUserIds = WebConfigurationManager.AppSettings["AdminUserIds"].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
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

        [HttpGet]
        public ActionResult Users()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Users(string searchTerm)
        {
            var users = UserManager.Users.Where(x => x.Email.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)).Select(x =>
                new FindUserResultViewModel { Email = x.Email, Id = x.Id, Name = x.UserName }).ToList();
            ViewBag.Users = users;
            return View("Users");
        }

        [HttpGet]
        public async Task<ActionResult> Impersonate(string id)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var user = await UserManager.FindByIdAsync(id);
            Session["ImpersonatingUser"] = HttpContext.User.Identity.GetUserId();
            await SignInManager.SignInAsync(user, false, false);
            return RedirectToAction("Index", "Manage");
        }
    }
}