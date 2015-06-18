using System;
using ManagementWeb.Areas.Hmac.Models;
using ManagementWeb.Areas.Hmac.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagementWeb.Tests.Repositories
{
    /// <summary>
    /// Note: Requires a locally running Hmac Management Service!
    /// Only use with a test database, test data is not rolled back!
    /// </summary>
    [TestClass]
    [Ignore]
    public class UserRepositoryIntegrationTest : GenericRepositoryIntegrationTest<UserRepository, UserModel, long?>
    {
        private const string UserServiceUrl = "http://localhost:14502/api/User";

        public UserRepositoryIntegrationTest()
        {
            ServiceUrl = UserServiceUrl;
        }

        protected override UserModel CreateElement()
        {
            return new UserModel
            {
                AppId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Enabled = true,
                Id = 1L,
                ServiceId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
        }

        protected override long? Parse(string result)
        {
            return long.Parse(result);
        }

        protected override bool IsIdValid(long? id)
        {
            return id != null && id > 0;
        }

        protected override void Disable(UserModel model)
        {
            model.Enabled = false;
        }
    }
}
