using System;
using System.Collections.Generic;
using System.Globalization;
using Dragon.Security.Hmac.Module.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Module.Tests.Repositories
{
    [TestClass]
    public class DapperAppRepositoryIntegrationTest : RepositoryIntegrationTestBase {

        private const string InsertAppScriptFilename = "insert_app.sql.st";

        [TestMethod]
        public void Get_appExists_shouldReturnApp()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, "Apps");
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Secret", secret },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
            });

            // Act
            var actual = repository.Get(appId, serviceId);

            // Assert
            Assert.AreEqual(appId, actual.AppId);
            Assert.AreEqual(serviceId, actual.ServiceId);
            Assert.AreEqual(secret, actual.Secret);
            Assert.AreEqual(createdAt.Date, actual.CreatedAt.Date);
            Assert.IsTrue(actual.Enabled);
            Assert.IsTrue(actual.Id > 0);
        }

        [TestMethod]
        public void Get_appDoesNotExist_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, "Apps");
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();

            // Act
            var actual = repository.Get(appId, serviceId);

            // Assert
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Get_appExistsOnlyForDifferentService_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, "Apps");
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", Guid.NewGuid().ToString() },
                { "Secret", secret },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
            });

            // Act
            var actual = repository.Get(appId, serviceId);

            // Assert
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Get_appDisabled_shouldReturnApp()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, "Apps");
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Secret", secret },
                { "Enabled", "0"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
            });

            // Act
            var actual = repository.Get(appId, serviceId);

            // Assert
            Assert.AreNotEqual(null, actual);
        }

    }
}
