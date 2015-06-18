using System;
using ManagementWeb.Areas.Hmac.Controllers;
using ManagementWeb.Areas.Hmac.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagementWeb.Tests.Controllers
{
    [TestClass]
    public class AppControllerTest : GenericControllerTest<AppController, AppModel, int?>
    {
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
    }
}
