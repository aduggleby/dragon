using System;
using ManagementWeb.Areas.Hmac.Controllers;
using ManagementWeb.Areas.Hmac.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagementWeb.Tests.Controllers
{
    [TestClass]
    public class UserControllerTest : GenericControllerTest<UserController, UserModel, long?>
    {
        protected override UserModel CreateElement()
        {
            return new UserModel
            {
                AppId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Enabled = true,
                Id = 1,
                ServiceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };
        }
    }
}
