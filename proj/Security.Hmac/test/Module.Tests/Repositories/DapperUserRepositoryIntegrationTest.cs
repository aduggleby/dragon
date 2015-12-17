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
    public class DapperUserRepositoryIntegrationTest : RepositoryIntegrationTestBase
    {

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
            var createdAt = DateTime.UtcNow;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userId.ToString()},
                {"AppId", appId.ToString()},
                {"ServiceId", serviceId.ToString()},
                {"Enabled", "1"},
                {"CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
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
        public void Get_byIdUserExists_shouldReturnUser()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            var userModel = new UserModel
            {
                UserId = userId,
                AppId = appId,
                ServiceId = serviceId,
                CreatedAt = createdAt,
                Enabled = true
            };

            // Act
            var id = repository.Insert(userModel);

            // Act
            var actual = repository.Get(id);

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
        public void Get_byIdUserDoesNotExist_shouldReturnNull()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);

            // Act
            var actual = repository.Get(23);

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
            var createdAt = DateTime.UtcNow;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userId.ToString()},
                {"AppId", appId.ToString()},
                {"ServiceId", Guid.NewGuid().ToString()},
                {"Enabled", "1"},
                {"CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
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
            var createdAt = DateTime.UtcNow;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userId.ToString()},
                {"AppId", appId.ToString()},
                {"ServiceId", serviceId.ToString()},
                {"Enabled", "0"},
                {"CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
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

            var userModel = CreateUser();


            // Act
            var id = repository.Insert(userModel);

            // Assert
            Assert.IsTrue(id > 0);
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
        [ExpectedException(typeof (NullReferenceException))]
        public void Insert_invalidUser_shouldThrowException()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);

            // Act
            repository.Insert(null);

            // Assert
            // see expected exception
        }

        [TestMethod]
        public void GetAll_userExist_shouldReturnUser()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userId.ToString()},
                {"AppId", appId.ToString()},
                {"ServiceId", serviceId.ToString()},
                {"Enabled", "1"},
                {"CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
            });
            var userId2 = Guid.NewGuid();
            var appId2 = Guid.NewGuid();
            var serviceId2 = Guid.NewGuid();
            var createdAt2 = DateTime.Now;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userId2.ToString()},
                {"AppId", appId2.ToString()},
                {"ServiceId", serviceId2.ToString()},
                {"Enabled", "1"},
                {"CreatedAt", createdAt2.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
            });

            // Act
            var actual = repository.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, actual.Count());

            var actualApp1 = actual.First(x => x.AppId == appId);
            Assert.AreEqual(userId, actualApp1.UserId);
            Assert.AreEqual(appId, actualApp1.AppId);
            Assert.AreEqual(serviceId, actualApp1.ServiceId);
            Assert.AreEqual(createdAt.Date, actualApp1.CreatedAt.Date);
            Assert.IsTrue(actualApp1.Enabled);
            Assert.IsTrue(actualApp1.Id > 0);

            var actualApp2 = actual.First(x => x.AppId == appId2);
            Assert.AreEqual(userId2, actualApp2.UserId);
            Assert.AreEqual(appId2, actualApp2.AppId);
            Assert.AreEqual(serviceId2, actualApp2.ServiceId);
            Assert.AreEqual(createdAt2.Date, actualApp2.CreatedAt.Date);
            Assert.IsTrue(actualApp2.Enabled);
            Assert.IsTrue(actualApp2.Id > 0);
        }

        [TestMethod]
        public void Delete_userExists_shouldRemoveUser()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userId.ToString()},
                {"AppId", appId.ToString()},
                {"ServiceId", serviceId.ToString()},
                {"Enabled", "1"},
                {"CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
            });
            var added = repository.Get(userId, serviceId);
            Assert.AreNotEqual(null, added);

            // Act
            repository.Delete(added.Id.Value);

            // Assert
            var actual = repository.Get(userId, serviceId);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void Delete_userDoesNotExist_shouldHaveNoImpact()
        {
            // Arrange
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var userId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            QueryFromFile(InsertUserScriptFilename, new Dictionary<string, string>
            {
                {"UserId", userId.ToString()},
                {"AppId", appId.ToString()},
                {"ServiceId", serviceId.ToString()},
                {"Enabled", "1"},
                {"CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
            });
            var added = repository.Get(userId, serviceId);
            var expected = repository.GetAll().Count();
            Assert.AreNotEqual(null, added);

            // Act
            repository.Delete(added.Id.Value + 1);

            // Assert
            var actual = repository.GetAll().Count();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Update_userExistsValidData_shouldUpdateUser()
        {
            var repository = new DapperUserRepository(Connection, UsersTableName);
            var user = CreateUser();
            user.Enabled = true;
            var id = repository.Insert(user);

            user = CreateUser();
            user.Enabled = false;
            repository.Update(id, user);

            var actual = repository.Get(id);
            user.Id = id;
            Assert.AreEqual(user, actual);
        }

        [TestMethod]
        [ExpectedException(typeof (NullReferenceException))]
        public void Update_invalidData_shouldThrowException()
        {
            var repository = new DapperUserRepository(Connection, UsersTableName);
            
            var user = CreateUser();
            var id = repository.Insert(user);

            repository.Update(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void Update_userDoesNotExist_shouldThrowException()
        {
            var repository = new DapperUserRepository(Connection, UsersTableName);
            
            var user = CreateUser();
            var id = repository.Insert(user);

            repository.Update(id + 1, user);
        }
        private static UserModel CreateUser()
        {
            return new UserModel
            {
                UserId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Enabled = true
            };
        }
    }
}
