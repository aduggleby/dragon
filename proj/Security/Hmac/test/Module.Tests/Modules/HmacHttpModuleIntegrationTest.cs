using System.Web;
using Dragon.Security.Hmac.Module.Modules;
using Dragon.Security.Hmac.Module.Services;
using Dragon.Security.Hmac.Module.Services.Validators;
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
            Assert.IsNotNull(actual.Validators);
            Assert.IsNotNull(actual.StatusCodes);
            Assert.IsNotNull(actual.Validators["appid"]);
            Assert.IsNotNull(((AppValidator)actual.Validators["appid"]).AppRepository);
            Assert.IsNotNull(((UserValidator)actual.Validators["userid"]).UserRepository);

            module.Dispose();
        }
    }
}
