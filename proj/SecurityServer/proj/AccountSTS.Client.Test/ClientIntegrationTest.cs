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
        private const string ApiUrl = "http://localhost:51385/Api/";
        private const string FederationUrl = "http://localhost:51385/";
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

        [TestMethod]
        public async Task Delete_validId_shouldDeleteAccount()
        {
            var client = CreateClient();
            await client.Delete(IntegrationTestHelper.ReadHmacSettings().UserId);
        }

        #region helper

        private static AccountSTSClient CreateClient()
        {
            var client = new AccountSTSClient(ApiUrl, FederationUrl, Realm);
            client.SetHmacSettings(IntegrationTestHelper.ReadHmacSettings());
            return client;
        }

        #endregion
    }
}
