using System.Web.Mvc;
using Common;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Controllers
{
    public class SetupController : Controller 
    {
        public ActionResult Index()
        {
            if (!SetupHelper.IsSetupAllowed()) return HttpNotFound();
            var repositorySetup = new RepositorySetup();
            repositorySetup.EnsureTableExists<AppMember>();
            repositorySetup.EnsureTableExists<IdentityUserClaim>();
            repositorySetup.EnsureTableExists<IdentityUserLogin>();
            repositorySetup.EnsureTableExists<IdentityUserService>();
            return Content("Setup complete.");
        }
    }
}