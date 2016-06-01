using System;
using System.Linq;
using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.SecurityServer.ProfileSTS.Client.Test
{
    [TestClass]
    public class ProfileSTSClientIntegrationTest
    {
        private const string ServiceUrl = "http://localhost:51386/api/ProfileApi/";
        private const string Realm = "http://WSFedTest/";

        [TestMethod]
        public async Task ClearCache_shouldClearCache()
        {
            var client = CreateClient();
            await client.ClearCache();
        }

        [TestMethod]
        public async Task Update_emailOnly_shouldUpdateEmail()
        {
            var client = CreateClient();
            var userId = IntegrationTestHelper.ReadHmacSettings().UserId;
            var value = "black" + Guid.NewGuid();
            const string color = "colorClaim";
            await client.AddClaim(userId, color, value);
            var actual = await client.GetClaims(userId);
            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Any(x => x.Type == color && x.Value == value));
        }

        #region helper

        private static ProfileSTSClient CreateClient()
        {
            var client = new ProfileSTSClient(ServiceUrl, Realm);
            client.SetHmacSettings(IntegrationTestHelper.ReadHmacSettings());
            return client;
        }

        #endregion
    }
}
