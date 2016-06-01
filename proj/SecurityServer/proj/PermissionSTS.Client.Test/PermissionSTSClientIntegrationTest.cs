using System.Linq;
using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient;
using Dragon.SecurityServer.GenericSTSClient.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.SecurityServer.PermissionSTS.Client.Test
{
    [TestClass]
    public class PermissionSTSClientIntegrationTest
    {
        private const string ServiceUrl = "http://localhost:51387/api/PermissionApi/";

        [TestMethod]
        public async Task ClearCache_shouldClearCache()
        {
            var client = CreateClient();
            await client.ClearCache();
        }

        [TestMethod]
        public async Task AddPermission_validData_shouldAddPermission()
        {
            var client = CreateClient();
            const string obj = "testData";
            const string operation = Operation.Read;
            var userId = IntegrationTestHelper.ReadHmacSettings().UserId;
            await client.AddPermission(userId, operation, obj);
            var actual = await client.GetClaims(userId);
            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Any(x => x.Type == operation && x.Value == obj));
        }

        [TestMethod]
        [ExpectedException(typeof(ApiException))]
        public async Task AddPermission_invalidSecret_shouldRefuseAccess()
        {
            var client = CreateClient();
            var settings = IntegrationTestHelper.ReadHmacSettings();
            settings.Secret = "invalid-secret";
            client.SetHmacSettings(settings);
            await client.GetClaims(IntegrationTestHelper.ReadHmacSettings().UserId);
        }

        #region helpers

        private static PermissionSTSClient CreateClient()
        {
            var client = new PermissionSTSClient(ServiceUrl);
            client.SetHmacSettings(IntegrationTestHelper.ReadHmacSettings());
            return client;
        }

        #endregion
    }
}
