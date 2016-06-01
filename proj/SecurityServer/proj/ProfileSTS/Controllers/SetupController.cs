using System.Web.Mvc;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Models;
using Dragon.SecurityServer.ProfileSTS.Models;

namespace Dragon.SecurityServer.ProfileSTS.Controllers
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
            return Content("Setup complete.");
        }
    }
}