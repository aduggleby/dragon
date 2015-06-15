using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services;
using Dragon.Security.Hmac.Module.Services.Validators;
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
        private const string DefaultSignatureParameterKey = "signature";

        [TestMethod]
        public void IsRequestAuthorized_rawUrlIsExcluded_shouldAllowAccess()
        {
            // Arrange
            var service = CreateHmacService(); 

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
            var service = new HmacHttpService(pathCollection, DefaultSignatureParameterKey)
            {
                Validators = CreateValidatorMap(ServiceId.ToString(), DefaultSignatureParameterKey, new Mock<IAppRepository>().Object, new Mock<IUserRepository>().Object, new HmacSha256Service()),
                StatusCodes = CreateStatusCodeMap(DefaultSignatureParameterKey)
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
            var service = CreateHmacService();

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateInvalidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.ParameterMissing, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_mismatchingServiceId_shouldDisallowRequest()
        {
            // Arrange
            var appRepository = new Mock<IAppRepository>();
            appRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new AppModel{Enabled = true});
            var service = new HmacHttpService(CreatePathCollection(), DefaultSignatureParameterKey)
            {
                Validators = CreateValidatorMap(Guid.NewGuid().ToString(), DefaultSignatureParameterKey, appRepository.Object, new Mock<IUserRepository>().Object, new HmacSha256Service()),
                StatusCodes = CreateStatusCodeMap(DefaultSignatureParameterKey)
            };

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
            Assert.AreEqual(StatusCode.InvalidOrDisabledAppId, actual);
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
            Assert.AreEqual(StatusCode.InvalidOrDisabledUserId, actual);
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
            Assert.AreEqual(StatusCode.InvalidOrDisabledServiceId, actual);
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
        public void IsRequestAuthorized_customSignatureKeyValidParameter_shouldAllowRequest()
        {
            // Arrange
            var service = CreateHmacService("s");

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString("s"));

            // Assert
            Assert.AreEqual(StatusCode.Authorized, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_usingHexEncodingValidParameter_shouldAllowRequest()
        {
            // Arrange
            var service = CreateHmacService("signature", true);

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString("signature", true));

            // Assert
            Assert.AreEqual(StatusCode.Authorized, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_userDoesntExistInDB_shouldStoreUserInDB()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((UserModel)null);
            var service = CreateHmacService(DefaultSignatureParameterKey, false, null, mockUserRepository.Object);

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
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((UserModel)null);
            var service = CreateHmacService(DefaultSignatureParameterKey, false, null, mockUserRepository.Object);

            // Act
            service.IsRequestAuthorized(GetValidRawUrl(), CreateInvalidQueryString());

            // Assert
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }

        [TestMethod]
        public void IsRequestAuthorized_validParameterButUserDisabled_shouldDisallowRequest()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new UserModel { Enabled = false });
            var service = CreateHmacService(DefaultSignatureParameterKey, false, null, mockUserRepository.Object);

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidOrDisabledUserId, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_userExistsInDB_shouldNotStoreUserInDB()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new UserModel());
            var service = CreateHmacService(DefaultSignatureParameterKey, false, null, mockUserRepository.Object);

            // Act
            service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }

        [TestMethod]
        public void IsRequestAuthorized_doesNotExistInvalidSignature_shouldNotStoreUserInDB()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((UserModel) null);
            var service = CreateHmacService(DefaultSignatureParameterKey, false, null, mockUserRepository.Object);

            // Act
            var queryString = CreateValidQueryString();
            queryString[DefaultSignatureParameterKey] = Guid.NewGuid().ToString();
            service.IsRequestAuthorized(GetValidRawUrl(), queryString);

            // Assert
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }

        [TestMethod]
        public void IsRequestAuthorized_validParameterButAppDisabled_shouldDisallowRequest()
        {
            // Arrange
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new AppModel { Enabled = false, Secret = Secret });
            var service = CreateHmacService(DefaultSignatureParameterKey, false, mockAppRepository.Object);

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidOrDisabledAppId, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_wrongSecret_shouldDisallowRequest()
        {
            // Arrange
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
                new AppModel { Enabled = true, Secret = Secret + "!" });
            var service = CreateHmacService(DefaultSignatureParameterKey, false, mockAppRepository.Object);

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidSignature, actual);
        }

        [TestMethod]
        public void IsRequestAuthorized_wrongAppId_shouldDisallowRequest()
        {
            // Arrange
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((AppModel)null);
            var service = CreateHmacService(DefaultSignatureParameterKey, false, mockAppRepository.Object);

            // Act
            var actual = service.IsRequestAuthorized(GetValidRawUrl(), CreateValidQueryString());

            // Assert
            Assert.AreEqual(StatusCode.InvalidOrDisabledAppId, actual);
        }

        #region helper

        private static HmacHttpService CreateHmacService(string signatureParameterKey = DefaultSignatureParameterKey, bool useHexEncoding = false, IAppRepository appRepository = null, IUserRepository userRepository = null)
        {
            if (appRepository == null)
            {
                appRepository = CreateMockAppRepository().Object;
            }
            if (userRepository == null)
            {
                userRepository = new Mock<IUserRepository>().Object;
            }
            var hmacService = signatureParameterKey == DefaultSignatureParameterKey
                ? new HmacSha256Service {UseHexEncoding = useHexEncoding}
                : new HmacSha256Service {SignatureParameterKey = signatureParameterKey, UseHexEncoding = useHexEncoding};
            var service = new HmacHttpService(CreatePathCollection(), signatureParameterKey)
            {
                Validators = CreateValidatorMap(ServiceId.ToString(), signatureParameterKey, appRepository, userRepository, hmacService),
                StatusCodes = CreateStatusCodeMap(signatureParameterKey)
            };
            return service;
        }

        public static Dictionary<string, IValidator> CreateValidatorMap(string serviceId, string signatureParameterKey, IAppRepository appRepository,
            IUserRepository userRepository, IHmacService hmacService)
        {
            var expiryValidator = new ExpiryValidator();
            var serviceValidator = new ServiceValidator(serviceId);
            var appValidator = new AppValidator
            {
                AppRepository = appRepository,
                ServiceValidator = serviceValidator
            };
            var hmacValidator = new HmacValidator
            {
                AppRepository = appRepository,
                AppValidator = appValidator,
                ServiceValidator = serviceValidator,
                HmacService = hmacService
            };
            var userValidator = new UserValidator
            {
                UserRepository = userRepository,
                AppValidator = appValidator,
                ServiceValidator = serviceValidator
            };

            return new Dictionary<string, IValidator>
            {
                {"expiry", expiryValidator},
                {"appid", appValidator},
                {"serviceid", serviceValidator},
                {"userid", userValidator},
                {signatureParameterKey, hmacValidator}
            };
        }

        public static Dictionary<string, StatusCode> CreateStatusCodeMap(string signatureParameterKey) {
            return new Dictionary<string, StatusCode>
            {
                {"expiry", StatusCode.InvalidExpiryOrExpired},
                {"appid", StatusCode.InvalidOrDisabledAppId},
                {"serviceid", StatusCode.InvalidOrDisabledServiceId},
                {"userid", StatusCode.InvalidOrDisabledUserId},
                {signatureParameterKey, StatusCode.InvalidSignature}
            };
        }
        
        private static Mock<IAppRepository> CreateMockAppRepository()
        {
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(new AppModel {Enabled = true, Secret = Secret});
            return mockAppRepository;
        }

        private static PathCollection CreatePathCollection()
        {
            return new PathCollection
            {
                new PathConfig {Name = "excluded", Path = "/public/.*", Type = PathConfig.PathType.Exclude},
                new PathConfig {Name = "included", Path = ".*", Type = PathConfig.PathType.Include}
            };
        }

        private static NameValueCollection CreateValidQueryString(string signatureParameterKey = DefaultSignatureParameterKey, bool useHexEncoding = false)
        {
            var queryString = new NameValueCollection
            {
                { "id", "23" },
                { "appid", AppId.ToString() },
                { "serviceid", ServiceId.ToString() },
                { "userid", UserId.ToString() },
                { "expiry", DateTime.Now.AddDays(+1).Ticks.ToString() }, 
            };
            var hmacService = signatureParameterKey == DefaultSignatureParameterKey ?
                new HmacSha256Service { UseHexEncoding = useHexEncoding } :
                new HmacSha256Service { SignatureParameterKey = signatureParameterKey, UseHexEncoding = useHexEncoding };
            queryString.Add(signatureParameterKey, hmacService.CalculateHash(hmacService.CreateSortedQueryString(queryString), Secret));
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
