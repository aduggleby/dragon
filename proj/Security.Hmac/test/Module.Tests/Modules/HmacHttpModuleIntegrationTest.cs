using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Dragon.Security.Hmac.Module.Modules;
using Dragon.Security.Hmac.Module.Services;
using Dragon.Security.Hmac.Module.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Module.Tests.Modules
{
    [TestClass]
    public class HmacHttpModuleIntegrationTest : TestBase
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

        [TestMethod]
        public void IsRequestAuthorized_validRequestToProtectedUrl_shouldAllowRequest()
        {
            // Arrange
            var module = new HmacHttpModule();
            module.Init(new HttpApplication());
            var service = (HmacHttpService) module.HmacHttpService;

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateQueryString());

            // Assert
            Assert.AreEqual(StatusCode.Authorized, actual);

            module.Dispose();
        }

        [TestMethod]
        public void IsRequestAuthorized_invalidRequestToProtectedUrl_shouldDisallowRequest()
        {
            // Arrange
            var module = new HmacHttpModule();
            module.Init(new HttpApplication());
            var service = (HmacHttpService) module.HmacHttpService;

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateInvalidQueryString());

            // Assert
            Assert.AreNotEqual(StatusCode.Authorized, actual);

            module.Dispose();
        }

        [TestMethod]
        public void IsRequestAuthorized_requestToUnprotectedUrl_shouldAllowRequest()
        {
            // Arrange
            var module = new HmacHttpModule();
            module.Init(new HttpApplication());
            var service = (HmacHttpService) module.HmacHttpService;

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(false), CreateInvalidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.Authorized, actual);

            module.Dispose();
        }

        #region helper

        private static NameValueCollection CreateQueryString()
        {
            return CreateValidQueryString(new Dictionary<string, string>
            {
                {"serviceid", "00000001-0001-0001-0001-000000000001"},
                {"appid", "00000001-0001-0001-0003-000000000001"},
            }, DefaultSignatureParameterKey, false, "secret");
        }

        #endregion
    }
}
