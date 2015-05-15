using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Module.Tests.Repositories
{
    [TestClass]
    public class DapperAppRepositoryIntegrationTest : RepositoryIntegrationTestBase {

        private const string InsertAppScriptFilename = "insert_app.sql.st";
        private const string AppTableName = "Apps";

        [TestMethod]
        public void Get_appExists_shouldReturnApp()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Secret", secret },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                { "Name", name },
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
            Assert.AreEqual(name, actual.Name);
        }

        [TestMethod]
        public void Get_appDoesNotExist_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();

            // Act
            var actual = repository.Get(appId, serviceId);

            // Assert
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Get_byIdAppExists_shouldReturnApp()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            var app = new AppModel
            {
                AppId = appId,
                ServiceId = serviceId,
                Secret = secret,
                Enabled = true,
                CreatedAt = createdAt,
                Name = name,
            };
            var id = repository.Insert(app);

            // Act
            var actual = repository.Get(id);

            // Assert
            Assert.AreEqual(appId, actual.AppId);
            Assert.AreEqual(serviceId, actual.ServiceId);
            Assert.AreEqual(secret, actual.Secret);
            Assert.AreEqual(createdAt.Date, actual.CreatedAt.Date);
            Assert.IsTrue(actual.Enabled);
            Assert.IsTrue(actual.Id > 0);
            Assert.AreEqual(name, actual.Name);
        }

        [TestMethod]
        public void Get_byIdAppDoesNotExist_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);

            // Act
            var actual = repository.Get(23);

            // Assert
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Get_appExistsOnlyForDifferentService_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", Guid.NewGuid().ToString() },
                { "Secret", secret },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                { "Name", name },
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
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Secret", secret },
                { "Enabled", "0"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                { "Name", name },
            });

            // Act
            var actual = repository.Get(appId, serviceId);

            // Assert
            Assert.AreNotEqual(null, actual);
        }

        [TestMethod]
        public void GetAll_appsExist_shouldReturnApps()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                {"AppId", appId.ToString()},
                {"ServiceId", serviceId.ToString()},
                {"Secret", secret},
                {"Enabled", "1"},
                {"CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
                { "Name", name },
            });
            var appId2 = Guid.NewGuid();
            var serviceId2 = Guid.NewGuid();
            var createdAt2 = DateTime.Now - TimeSpan.FromSeconds(23);
            const string secret2 = "testsecret2";
            const string name2 = "user2";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                {"AppId", appId2.ToString()},
                {"ServiceId", serviceId2.ToString()},
                {"Secret", secret2},
                {"Enabled", "1"},
                {"CreatedAt", createdAt2.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
                { "Name", name2 },
            });

            // Act
            var actual = repository.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, actual.Count());

            var actualApp1 = actual.First(x => x.AppId == appId);
            Assert.AreEqual(serviceId, actualApp1.ServiceId);
            Assert.AreEqual(secret, actualApp1.Secret);
            Assert.AreEqual(createdAt.Date, actualApp1.CreatedAt.Date);
            Assert.AreEqual(name, actualApp1.Name);
            Assert.IsTrue(actualApp1.Enabled);

            var actualApp2 = actual.First(x => x.AppId == appId2);
            Assert.AreEqual(serviceId2, actualApp2.ServiceId);
            Assert.AreEqual(secret2, actualApp2.Secret);
            Assert.AreEqual(createdAt2.Date, actualApp2.CreatedAt.Date);
            Assert.AreEqual(name2, actualApp2.Name);
            Assert.IsTrue(actualApp2.Enabled);
        }

        [TestMethod]
        public void Delete_appExists_shouldRemoveApp()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Secret", secret },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                { "Name", name },
            });
            var added = repository.Get(appId, serviceId);
            Assert.AreNotEqual(null, added);

            // Act
            repository.Delete(added.Id.Value);

            // Assert
            var actual = repository.Get(appId, serviceId);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Delete_appDoesNotExists_shouldHaveNoImpact()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            QueryFromFile(InsertAppScriptFilename, new Dictionary<string, string>
            {
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Secret", secret },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                { "Name", name },
            });
            var added = repository.Get(appId, serviceId);
            Assert.AreNotEqual(null, added);
            var expected = repository.GetAll().Count();

            // Act
            repository.Delete(added.Id.Value + 1);

            // Assert
            var actual = repository.GetAll().Count();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Insert_validData_shouldInsertApp()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            const string secret = "testsecret";
            const string name = "user1";
            var app = new AppModel
            {
                AppId = appId,
                ServiceId = serviceId,
                Secret = secret,
                Enabled = true,
                CreatedAt = createdAt,
                Name = name,
            };

            // Act
            var id = repository.Insert(app);

            // Assert
            Assert.IsTrue(id > 0);
            var actual = repository.Get(appId, serviceId);
            Assert.AreEqual(appId, actual.AppId);
            Assert.AreEqual(serviceId, actual.ServiceId);
            Assert.AreEqual(secret, actual.Secret);
            Assert.AreEqual(createdAt.Date, actual.CreatedAt.Date);
            Assert.AreEqual(name, actual.Name);
            Assert.IsTrue(actual.Enabled);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Insert_dataIsNull_shouldNotInsertApp()
        {
            // Arrange
            var repository = new DapperAppRepository(Connection, AppTableName);

            // Act
            repository.Insert(null);

            // Assert
            // see expected exception
        }

        [TestMethod]
        public void Update_appExistsValidData_shouldUpdateApp()
        {
            var repository = new DapperAppRepository(Connection, AppTableName);
            var app = CreateApp();
            app.Enabled = true;
            var id = repository.Insert(app);

            app = CreateApp();
            app.Enabled = false;
            repository.Update(id, app);

            var actual = repository.Get(id);
            app.Id = id;
            Assert.AreEqual(app, actual);
        }

        [TestMethod]
        [ExpectedException(typeof (NullReferenceException))]
        public void Update_invalidData_shouldThrowException()
        {
            var repository = new DapperAppRepository(Connection, AppTableName);
            
            var app = CreateApp();
            var id = repository.Insert(app);

            repository.Update(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void Update_appDoesNotExist_shouldThrowException()
        {
            var repository = new DapperAppRepository(Connection, AppTableName);
            
            var app = CreateApp();
            var id = repository.Insert(app);

            repository.Update(id + 1, app);
        }

        private static AppModel CreateApp()
        {
            return new AppModel
            {
                AppId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                Secret = "testsecret",
                Enabled = true,
                CreatedAt = DateTime.Now,
                Name = "name_" + Guid.NewGuid()
            };
        }
    }
}
