using System;
using System.Linq;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using ManagementService.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ManagementService.Tests.Controllers
{
    [TestClass]
    public class AppControllerTest
    {
        [TestMethod]
        public void Get_allAppsExist_shouldReturnApps()
        {
            var expected = CreateApp();
            var mockRepository = new Mock<IAppRepository>();
            mockRepository.Setup(x => x.GetAll()).Returns(new[]{ expected, expected }.ToList());
            var controller = new AppController {AppRepository = mockRepository.Object};

            var actual = controller.Get();

            Assert.AreEqual(expected, actual.First());
        }

        [TestMethod]
        public void Get_appExists_shouldReturnApp()
        {
            var expected = CreateApp();
            var mockRepository = new Mock<IAppRepository>();
            mockRepository.Setup(x => x.Get(expected.Id.Value)).Returns(expected);
            var controller = new AppController {AppRepository = mockRepository.Object};

            var actual = controller.Get(expected.Id.Value);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Post_validModel_shouldInsertModel()
        {
            var newApp = CreateApp();
            var mockRepository = new Mock<IAppRepository>();
            var controller = new AppController {AppRepository = mockRepository.Object};

            controller.Post(newApp);

            mockRepository.Verify(x => x.Insert(newApp));
        }

        [TestMethod]
        public void Delete_existingModel_shouldRemoveModel()
        {
            var newApp = CreateApp();
            var mockRepository = new Mock<IAppRepository>();
            var controller = new AppController {AppRepository = mockRepository.Object};

            controller.Delete(newApp.Id.Value);

            mockRepository.Verify(x => x.Delete(newApp.Id.Value));
        }

        # region helpers

        private static AppModel CreateApp()
        {
            return new AppModel
            {
                AppId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Enabled = true,
                Id = 1,
                ServiceId = Guid.NewGuid(),
                Name = "test_app1",
                Secret = "secret_app1"
            };
        }

        # endregion
    }
}
