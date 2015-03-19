using System.Web;
using Dragon.Security.Hmac.Module.Modules;
using Dragon.Security.Hmac.Module.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Module.Tests.Modules
{
    [TestClass]
    public class HmacHttpModuleIntegrationTest
    {
        [TestMethod]
        public void Init_noHttpServiceSet_shouldCreateDefaultService()
        {
            // Arrange
            var module = new HmacHttpModule();

            // Act
            module.Init(new HttpApplication());

            // Assert
            var actual = (HmacHttpService) module.HmacHttpService;
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.UserRepository);
            Assert.IsNotNull(actual.AppRepository);
            Assert.IsNotNull(actual.HmacService);

            module.Dispose();
        }
    }
}
