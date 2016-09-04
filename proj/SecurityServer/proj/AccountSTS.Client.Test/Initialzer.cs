using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.GenericSTSClient.Test;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Client.Test
{
    [TestClass]
    //[Ignore]
    public class Initializer
    {
        private const string TargetDbPath = @"\..\..\..\AccountSTS\App_Data\aspnet-AccountSTS-Dragon.mdf";
        private const string TestDbName = "AccountSTSClientTest";
        private const string LoginProvider = "TestProvider";

        [TestMethod]
        public void InitializeDatabase()
        {
            var hmacSettings = IntegrationTestHelper.ReadHmacSettings();
            IntegrationTestInitializer.DatabaseCallback = conn =>
            {
                conn.CreateTableIfNotExists<AppMember>();
                conn.CreateTableIfNotExists<IdentityUserClaim>();
                conn.CreateTableIfNotExists<IdentityUserLogin>();
                conn.CreateTableIfNotExists<IdentityUserService>();
            };
            IntegrationTestInitializer.TestDataCallback = (sid, uid, aid, dbn, sec) =>
            {
                var userRepository = new Repository<AppMember>();
                var email = Guid.NewGuid().ToString() + "@test.local";
                userRepository.Insert(new AppMember { Id = hmacSettings.UserId, UserName = email, Email = email});
                var loginRepository = new Repository<IdentityUserLogin>();
                loginRepository.Insert(new IdentityUserLogin
                {
                    LoginProvider = LoginProvider,
                    ProviderKey = Guid.NewGuid().ToString(),
                    UserId = hmacSettings.UserId 
                });
            };
            IntegrationTestInitializer.CreateTestDatabase(new Guid(hmacSettings.ServiceId), new Guid(hmacSettings.UserId), new Guid(hmacSettings.AppId), TestDbName, hmacSettings.Secret, TargetDbPath);
        }
    }
}
