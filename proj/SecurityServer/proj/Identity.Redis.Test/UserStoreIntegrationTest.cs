using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.AspNet.Identity;

namespace Dragon.SecurityServer.Identity.Redis.Test
{
    /// <summary>
    /// Note: Requires a running Redis db on localhost!
    /// </summary>
    [TestClass]
    public class UserStoreIntegrationTest
    {
        [TestMethod]
        public async Task CreateAsync_validUser_shouldStoreUser()
        {
            var userStore = CreateUserStore();
            var id = Guid.NewGuid().ToString();
            await CreateUser(userStore, id);
            var user = await userStore.FindByIdAsync(id);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task UpdateAsync_validUser_shouldUpdateModel()
        {
            var userStore = CreateUserStore();
            var id = Guid.NewGuid().ToString();
            await CreateUser(userStore, id);
            const string email = "test2@test.local";
            const string userName = "test3@test.local";

            await Task.Run(async () => await userStore.UpdateAsync(new IdentityUser
            {
                Id = id,
                Email = email,
                UserName = "test3@test.local"
            }));

            var users = new[] {await userStore.FindByIdAsync(id), await userStore.FindByNameAsync(userName)};
            foreach (var user in users)
            {
                Assert.IsNotNull(user);
                Assert.AreEqual(email, user.Email);
                Assert.AreEqual(userName, user.UserName);
            }
        }

        [TestMethod]
        public async Task DeleteAllAsync_shouldSucceed()
        {
            var userStore = CreateUserStore();
            IList<string> ids = new List<string>();
            for (var i = 0; i < 5; i++)
            {
                var id = Guid.NewGuid().ToString();
                await CreateUser(userStore, id);
                ids.Add(id);
            }
            await userStore.DeleteAllAsync();

            foreach (var id in ids)
            {
                Assert.IsNull(await userStore.FindByIdAsync(id));
            }
        }

        [TestMethod]
        public async Task ClearCache_shouldDoNothing()
        {
            var userStore = CreateUserStore();
            IList<string> ids = new List<string>();
            for (var i = 0; i < 5; i++)
            {
                var id = Guid.NewGuid().ToString();
                await CreateUser(userStore, id);
                ids.Add(id);
            }
            await Task.Run(async () => await userStore.ClearCache());

            foreach (var id in ids)
            {
                Assert.IsNotNull(await userStore.FindByIdAsync(id));
            }
        }

        [TestMethod]
        public async Task AddLoginToUserAsync_validUserAndLogin_shouldSucceed()
        {
            var userStore = CreateUserStore();
            var id = Guid.NewGuid().ToString();
            await CreateUser(userStore, id);
            var user = await userStore.FindByIdAsync(id);

            var loginProvider = "provider";
            var providerKey = "key" + id;
            await userStore.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey));

            var actual = await userStore.GetLoginsAsync(user);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(actual[0].LoginProvider, loginProvider);
            Assert.AreEqual(actual[0].ProviderKey, providerKey);
        }

        [TestMethod]
        public async Task AddServiceToUserAsync_validService_shouldSucceed()
        {
            var userStore = CreateUserStore();
            var id = Guid.NewGuid().ToString();
            await CreateUser(userStore, id);
            var user = await userStore.FindByIdAsync(id);

            var serviceId = Guid.NewGuid().ToString();
            await userStore.AddServiceToUserAsync(user, serviceId);

            var actual = (await userStore.GetServicesAsync(user)).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(actual[0], serviceId);
        }

        [TestMethod]
        public async Task IsUserRegisteredForServiceAsync_userRegistered_shouldReturnTrue()
        {
            var userStore = CreateUserStore();
            var id = Guid.NewGuid().ToString();
            await CreateUser(userStore, id);
            var user = await userStore.FindByIdAsync(id);

            var serviceId = Guid.NewGuid().ToString();
            await userStore.AddServiceToUserAsync(user, serviceId);

            var actual = await userStore.IsUserRegisteredForServiceAsync(user, serviceId);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public async Task IsUserRegisteredForServiceAsync_userNotRegistered_shouldReturnFalse()
        {
            var userStore = CreateUserStore();
            var id = Guid.NewGuid().ToString();
            await CreateUser(userStore, id);
            var user = await userStore.FindByIdAsync(id);

            var serviceId = Guid.NewGuid().ToString();

            var actual = await userStore.IsUserRegisteredForServiceAsync(user, serviceId);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public async Task GetServicesAsync_userWithServices_shouldReturnServices()
        {
            var userStore = CreateUserStore();
            var id = Guid.NewGuid().ToString();
            await CreateUser(userStore, id);
            var user = await userStore.FindByIdAsync(id);
            var serviceIds = new[] {Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
            foreach (var serviceId in serviceIds)
            {
                await userStore.AddServiceToUserAsync(user, serviceId);
            }

            var actual = (await userStore.GetServicesAsync(user)).ToList();

            foreach (var serviceId in serviceIds)
            {
                Assert.IsTrue(actual.Contains(serviceId));
            }
        }

        #region Helper

        private static async Task CreateUser(IUserStore<IdentityUser, string> userStore, string id)
        {
            await Task.Run(async () => await userStore.CreateAsync(new IdentityUser
            {
                Id = id,
                Email = CreateEmailFromId(id),
                UserName = CreateNameFromId(id)
            }));
        }

        private static string CreateNameFromId(string id)
        {
            return string.Format("u{0}@test.local", id);
        }

        private static string CreateEmailFromId(string id)
        {
            return string.Format("e{0}@test.local", id);
        }

        private static UserStore<IdentityUser> CreateUserStore()
        {
            var multiplexer = ConnectionMultiplexer.Connect("localhost");
            var store = new RedisUserStore<IdentityUser>(multiplexer);
            var userStore = new UserStore<IdentityUser>(store, multiplexer);
            return userStore;
        }

        #endregion
    }
}
