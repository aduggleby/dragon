using System;
using System.Threading.Tasks;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.GenericSTSClient.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.SecurityServer.AccountSTS.Client.Test
{
    [TestClass]
    public class ClientIntegrationTest
    {
        private const string ServiceUrl = "http://thispc.com:51385/api/AccountApi/";
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
            await client.Update(new UpdateViewModel
            {
                Id = IntegrationTestHelper.ReadHmacSettings().UserId,
                Email = "e" + Guid.NewGuid().ToString().Substring(0, 8) + "@localhost.local"
            });
        }

        [TestMethod]
        public async Task ResetPassword_emailOnly_shouldUpdateEmail()
        {
            var client = CreateClient();
            var password = Guid.NewGuid().ToString();
            var email = "e" + Guid.NewGuid().ToString().Substring(0, 8) + "@localhost.local";
            await client.Update(new UpdateViewModel
            {
                Id = IntegrationTestHelper.ReadHmacSettings().UserId,
                Password = password,
                ConfirmPassword = password,
                Email = email
            });

            var newPassword = Guid.NewGuid() + "A.a.1";

            await client.ResetPassword(new ResetPasswordViewModel
            {
                Email = email,
                Code = Guid.NewGuid().ToString(),
                Password = newPassword,
                ConfirmPassword = newPassword
            });
        }

        #region helper

        private static AccountSTSClient CreateClient()
        {
            var client = new AccountSTSClient(ServiceUrl, Realm);
            client.SetHmacSettings(IntegrationTestHelper.ReadHmacSettings());
            return client;
        }

        #endregion
    }
}
