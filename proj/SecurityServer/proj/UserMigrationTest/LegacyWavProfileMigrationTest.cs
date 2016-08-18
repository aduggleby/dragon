using System.Configuration;
using System.Threading.Tasks;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.Identity.Models;
using Dragon.SecurityServer.Identity.Stores;
using Dragon.SecurityServer.ProfileSTS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserMigration;

namespace UserMigrationTest
{
    [TestClass]
    public class LegacyWavProfileMigrationTest
    {
        [TestMethod]
        public async Task Migrate_validDbs_shouldMigrateProfiles()
        {
            ConnectionHelper.ConnectionString = () => ConfigurationManager.ConnectionStrings["ProfileSts"].ConnectionString;
            var userStore = new UserStore<AppMember>(new Repository<AppMember>(), new Repository<IdentityUserClaim>(), new Repository<IdentityUserLogin>(), new Repository<IdentityUserService>(), new Repository<IdentityUserApp>());
            var service = new LegacyWavProfileMigration<AppMember>(userStore);
            await service.Migrate();
        }
    }
}
