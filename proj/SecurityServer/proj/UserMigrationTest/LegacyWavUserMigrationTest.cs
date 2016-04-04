using System.Threading.Tasks;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserMigration;

namespace UserMigrationTest
{
    [TestClass]
    public class LegacyWavUserMigrationTest
    {
        [TestMethod]
        public async Task Migrate_validDbs_shouldMigrateUsers()
        {
            var service = new LegacyWavUserMigration<AppMember>(
                new UserStore<AppMember>(new Repository<AppMember>(), new Repository<IdentityUserClaim>(), new Repository<IdentityUserLogin>(), new Repository<IdentityService>()));
            await service.Migrate(data => new AppMember { Id = data.UserID.ToString(), Email = data.Email, PasswordHash = data.Secret });
        }
    }
}
