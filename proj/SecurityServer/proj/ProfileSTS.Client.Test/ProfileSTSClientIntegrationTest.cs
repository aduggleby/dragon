using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.SecurityServer.ProfileSTS.Client.Test
{
    [TestClass]
    public class ProfileSTSClientIntegrationTest
    {
        private const string ServiceUrl = "http://localhost:51386/api/";
        private const string Realm = "http://WSFedTest/";

        [TestMethod]
        public async Task ClearCache_shouldClearCache()
        {
            var client = CreateClient();
            await client.ClearCache();
        }

        [TestMethod]
        public async Task AddClaim_validData_shouldAddClaim()
        {
            var client = CreateClient();
            var userId = IntegrationTestHelper.ReadHmacSettings().UserId;
            var value = "black" + Guid.NewGuid();
            var type = "colorClaim" + Guid.NewGuid();
            await client.AddClaim(userId, type, value);
            var actual = await client.GetClaims(userId);
            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Any(x => x.Type == type && x.Value == value));
        }

        [TestMethod]
        public async Task UpdateClaim_validData_shouldUpdateClaim()
        {
            var client = CreateClient();
            var userId = IntegrationTestHelper.ReadHmacSettings().UserId;
            var value = "black" + Guid.NewGuid();
            var type = "colorClaim" + Guid.NewGuid();
            await client.AddClaim(userId, type, value);
            value = "black" + Guid.NewGuid();
            await client.UpdateClaim(userId, type, value);
            var actual = await client.GetClaims(userId);
            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Any(x => x.Type == type && x.Value == value));
        }

        [TestMethod]
        public async Task RemoveClaim_claimExists_shouldRemoveClaim()
        {
            var client = CreateClient();
            var userId = IntegrationTestHelper.ReadHmacSettings().UserId;
            var value = "black" + Guid.NewGuid();
            var type = "colorClaim" + Guid.NewGuid();
            await client.AddClaim(userId, type, value);
            await client.RemoveClaim(userId, type);
            var actual = await client.GetClaims(userId);
            Assert.IsFalse(actual.Any(x => x.Type == type && x.Value == value));
        }

        [TestMethod]
        public async Task AddOrUpdateClaims_validData_shouldAddOrUpdateClaims()
        {
            var client = CreateClient();
            var userId = IntegrationTestHelper.ReadHmacSettings().UserId;
            var value = "black" + Guid.NewGuid();
            var type = "colorClaim" + Guid.NewGuid();
            await client.AddClaim(userId, type, value);
            var claims = new List<Claim> {new Claim(type, "black" + Guid.NewGuid()), new Claim("material" + Guid.NewGuid(), "metal" + Guid.NewGuid())};
            await client.AddOrUpdateClaims(userId, claims);
            var actual = await client.GetClaims(userId);
            foreach (var expectedClaim in claims)
            {
                Assert.IsTrue(actual.Any(x => x.Type == expectedClaim.Type && x.Value == expectedClaim.Value));
            }
        }

        [TestMethod]
        public async Task Delete_validId_shouldDeleteAccount()
        {
            var client = CreateClient();
            await client.Delete(IntegrationTestHelper.ReadHmacSettings().UserId);
        }

        #region helper

        private static ProfileSTSClient CreateClient()
        {
            var client = new ProfileSTSClient(ServiceUrl);
            client.SetHmacSettings(IntegrationTestHelper.ReadHmacSettings());
            return client;
        }

        #endregion
    }
}

