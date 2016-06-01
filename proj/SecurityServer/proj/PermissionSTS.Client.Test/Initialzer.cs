using System;
using Dapper;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.GenericSTSClient.Test;
using Dragon.SecurityServer.Identity.Models;
using Dragon.SecurityServer.PermissionSTS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.SecurityServer.PermissionSTS.Client.Test
{
    [TestClass]
    //[Ignore]
    public class Initializer
    {
        private const string TargetDbPath = @"\..\..\..\PermissionSTS\App_Data\aspnet-PermissionSTS-Dragon.mdf";
        private const string TestDbName = "PermissionSTSClientTest";
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
            };
            IntegrationTestInitializer.TestDataCallback = (sid, uid, aid, dbn, sec) =>
            {
                var userRepository = new Repository<AppMember>();
                userRepository.Insert(new AppMember { Id = hmacSettings.UserId });
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
