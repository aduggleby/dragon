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
    public class DapperUserRepositoryIntegrationTest : RepositoryIntegrationTestBase {

        private const string InsertUserScriptFilename = "insert_user.sql.st";
        private const string SelectUserScriptFilename = "select_user.sql.st";

        private const string UsersTableName = "Users";

        [TestMethod]
        public void Get_userExists_shouldReturnUser()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
            });

            // Act
            var actual = repository.Get(userId, serviceId);

            // Assert
            Assert.AreEqual(userId, actual.UserId);
            Assert.AreEqual(appId, actual.AppId);
            Assert.AreEqual(serviceId, actual.ServiceId);
            Assert.AreEqual(createdAt.Date, actual.CreatedAt.Date);
            Assert.IsTrue(actual.Enabled);
            Assert.IsTrue(actual.Id > 0);
        }

        [TestMethod]
        public void Get_userDoesNotExist_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();

            // Act
            var actual = repository.Get(userId, serviceId);

            // Assert
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Get_userExistsOnlyForDifferentService_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "AppId", appId.ToString() },
                { "ServiceId", Guid.NewGuid().ToString() },
                { "Enabled", "1"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
            });

            // Act
            var actual = repository.Get(userId, serviceId);

            // Assert
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Get_userDisabled_shouldReturnUser()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "AppId", appId.ToString() },
                { "ServiceId", serviceId.ToString() },
                { "Enabled", "0"},
                { "CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
            });

            // Act
            var actual = repository.Get(userId, serviceId);

            // Assert
            Assert.AreNotEqual(null, actual);
        }

        [TestMethod]
        public void Insert_validUser_shouldInsertUser()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userModel = new UserModel
            {
                UserId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Enabled = true
            };

            // Act
            repository.Insert(userModel);

            // Assert
            var users = QueryFromFile<UserModel>(SelectUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userModel.UserId.ToString()}
            }).ToList();
            Assert.AreNotEqual(0, users.Count());
            var actual = users.First();
            Assert.AreEqual(userModel.UserId, actual.UserId);
            Assert.AreEqual(userModel.AppId, actual.AppId);
            Assert.AreEqual(userModel.ServiceId, actual.ServiceId);
            Assert.AreEqual(userModel.CreatedAt.Date, actual.CreatedAt.Date);
            Assert.IsTrue(actual.Enabled);
            Assert.IsTrue(actual.Id > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Insert_invalidUser_shouldThrowException()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);

            // Act
            repository.Insert(null);

            // Assert
            // see expected exception
        }
    }
}
