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
    public class AppRepositoryIntegrationTest : GenericRepositoryIntegrationTest<AppRepository, AppModel, int?>
    {
        private const string AppServiceUrl = "http://localhost:14502/api/App";

        public AppRepositoryIntegrationTest()
        {
            ServiceUrl = AppServiceUrl;
        }

        protected override AppModel CreateElement()
        {
            return new AppModel
            {
                AppId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Enabled = true,
                Id = 1,
                ServiceId = Guid.NewGuid(),
                Name = "name_" + Guid.NewGuid(),
                Secret = "secret_" + Guid.NewGuid()
            };
        }

        protected override int? Parse(string result)
        {
            return int.Parse(result);
        }

        protected override bool IsIdValid(int? id)
        {
            return id != null && id > 0;
        }

        protected override void Disable(AppModel model)
        {
            model.Enabled = false;
        }
    }
}
