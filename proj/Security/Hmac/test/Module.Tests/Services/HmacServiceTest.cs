using System;
using System.Collections.Specialized;
using System.Globalization;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Security.Hmac.Module.Tests.Services
{
    [TestClass]
    public class HmacServiceTest
    {
        private static readonly Guid AppId = Guid.NewGuid();
        private static readonly Guid ServiceId = Guid.NewGuid();
        private static readonly Guid UserId = Guid.NewGuid();
        private const string Secret = "%DF47hf*hdf";

        [TestMethod]
        public void IsRequestAuthorized_rawUrlIsExcluded_shouldAllowAccess()
        {
            // Arrange
            var service = new HmacHttpService(ServiceId.ToString(), CreatePathCollection())
            {
                UserRepository = new Mock<IUserRepository>().Object,
                AppRepository = new Mock<IAppRepository>().Object,
                HmacService = new HmacSha256Service()
            };

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(false), CreateInvalidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.Authorized, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_rawUrlIsExcludedButIncludedBeforeThat_shouldDisallowRequest()
        {
            // Arrange
            var pathCollection = new PathCollection
            {
                new PathConfig {Name = "included", Path = ".*", Type = PathConfig.PathType.Include},
                new PathConfig {Name = "excluded", Path = "/public/.*", Type = PathConfig.PathType.Exclude}
            };
            var service = new HmacHttpService(ServiceId.ToString(), pathCollection)
            {
                UserRepository = new Mock<IUserRepository>().Object,
                AppRepository = new Mock<IAppRepository>().Object,
                HmacService = new HmacSha256Service()
            };

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(false), CreateInvalidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.ParameterMissing, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_rawUrlIsIncludedInvalidQueryString_shouldDisallowRequest()
        {
            // Arrange
            var service = new HmacHttpService(ServiceId.ToString(), CreatePathCollection())
            {
                UserRepository = new Mock<IUserRepository>().Object,
                AppRepository = new Mock<IAppRepository>().Object,
                HmacService = new HmacSha256Service()
            };

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateInvalidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.ParameterMissing, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_mismatchingServiceId_shouldDisallowRequest()
        {
            // Arrange
            var service = new HmacHttpService(Guid.NewGuid().ToString(), CreatePathCollection());

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidOrDisabledServiceId, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_missingParameter_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateInvalidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.ParameterMissing, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_invalidAppId_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var queryString = CreateValidQueryString();
            queryString["appid"] = "23";

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), queryString);

            // Assert
            Assert.AreEqual(StatusCode.InvalidParameterFormat, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_invalidUserId_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();  
            var queryString = CreateValidQueryString();
            queryString["userid"] = "23";

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), queryString);

            // Assert
            Assert.AreEqual(StatusCode.InvalidParameterFormat, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_invalidServiceId_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var queryString = CreateValidQueryString();
            queryString["serviceid"] = "23";

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), queryString);

            // Assert
            Assert.AreEqual(StatusCode.InvalidParameterFormat, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_invalidExpiry_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var queryString = CreateValidQueryString();
            queryString["expiry"] = DateTime.Now.AddDays(-1).Ticks.ToString();

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), queryString);

            // Assert
            Assert.AreEqual(StatusCode.InvalidExpiryOrExpired, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_malformedExpiry_shouldReturnDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var queryString = CreateValidQueryString();
            queryString["expiry"] = DateTime.Now.AddDays(-1).ToString(CultureInfo.InvariantCulture);

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), queryString);

            // Assert
            Assert.AreEqual(StatusCode.InvalidExpiryOrExpired, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_invalidSignature_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var queryString = CreateValidQueryString();
            queryString["signature"] = Guid.NewGuid().ToString();

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), queryString);

            // Assert
            Assert.AreEqual(StatusCode.InvalidSignature, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_validParameter_shouldAllowRequest()
        {
            // Arrange
            var service = CreateHmacService();

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.Authorized, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_userDoesntExistInDB_shouldStoreUserInDB()
        {
            // Arrange
            var service = CreateHmacService();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((UserModel)null);
            service.UserRepository = mockUserRepository.Object;

            // Act
            service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            mockUserRepository.Verify(x => x.Insert(It.Is<UserModel>(
                y => y.AppId == AppId && y.ServiceId == ServiceId && y.UserId == UserId && y.Enabled && y.CreatedAt.Date == DateTime.Now.Date
                )), Times.Once);
        }

        [TestMethod]
        public void IsRequestAuthorized_userDoesntExistInDBInvalidRequest_shouldNotStoreUserInDB()
        {
            // Arrange
            var service = CreateHmacService();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((UserModel)null);
            service.UserRepository = mockUserRepository.Object;

            // Act
            service.IsRequestAuthorized(GetValidRawUrl(), CreateInvalidQueryString());

            // Assert
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }

        [TestMethod]
        public void IsRequestAuthorized_validParameterButUserDisabled_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new UserModel { Enabled = false });
            service.UserRepository = mockUserRepository.Object;

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidOrDisabledUserId, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_userExistsInDB_shouldNotStoreUserInDB()
        {
            // Arrange
            var service = CreateHmacService();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new UserModel());
            service.UserRepository = mockUserRepository.Object;

            // Act
            service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }

        [TestMethod]
        public void IsRequestAuthorized_validParameterButAppDisabled_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new AppModel { Enabled = false, Secret = Secret });
            service.AppRepository = mockAppRepository.Object;

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidOrDisabledAppId, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_wrongSecret_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
                new AppModel { Enabled = true, Secret = Secret + "!" });
            service.AppRepository = mockAppRepository.Object;

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidSignature, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_wrongAppId_shouldDisallowRequest()
        {
            // Arrange
            var service = CreateHmacService();
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((AppModel)null);
            service.AppRepository = mockAppRepository.Object;

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidOrDisabledAppId, actual);
        }

        #region helper

        private static HmacHttpService CreateHmacService()
        {
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new AppModel { Enabled = true, Secret = Secret });
            return new HmacHttpService(ServiceId.ToString(), CreatePathCollection())
            {
                UserRepository = new Mock<IUserRepository>().Object,
                AppRepository = mockAppRepository.Object,
                HmacService = new HmacSha256Service()
            };
        }

        private static PathCollection CreatePathCollection()
        {
            return new PathCollection
            {
                new PathConfig {Name = "excluded", Path = "/public/.*", Type = PathConfig.PathType.Exclude},
                new PathConfig {Name = "included", Path = ".*", Type = PathConfig.PathType.Include}
            };
        }

        private static NameValueCollection CreateValidQueryString()
        {
            var queryString = new NameValueCollection
            {
                { "id", "23" },
                { "appid", AppId.ToString() },
                { "serviceid", ServiceId.ToString() },
                { "userid", UserId.ToString() },
                { "expiry", DateTime.Now.AddDays(+1).Ticks.ToString() }, 
            };
            var hmacService = new HmacSha256Service();
            queryString.Add("signature", hmacService.CalculateHash(hmacService.CreateSortedQueryValuesString(queryString), Secret));
            return queryString;
        }

        private static NameValueCollection CreateInvalidQueryString()
        {
            return new NameValueCollection { { "id", "23" } };
        }

        private static string GetValidRawUrl(bool secret = true)
        {
            var dir = !secret ? "public" : "private";
            return String.Format("http://localhost/{0}/index.html", dir);
        }

        #endregion
    }
}
