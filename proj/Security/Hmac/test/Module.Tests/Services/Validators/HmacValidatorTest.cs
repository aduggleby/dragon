using System;
using System.Collections.Specialized;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Security.Hmac.Module.Tests.Services.Validators
{
    [TestClass]
    public class HmacValidatorTest
    {
        [TestMethod]
        [ExpectedException(typeof (NotYetParsedException))]
        public void GetValue_notYetParsed_shouldThrowException()
        {
            new HmacValidator().GetValue();
        }

        [TestMethod]
        public void GetValue_alreadyParsed_shouldReturnValue()
        {
            var data = Guid.NewGuid().ToString();
            var validator = new HmacValidator();
            validator.Parse(data);

            var actual = (string) validator.GetValue();

            Assert.AreEqual(data, actual);
        }

        [TestMethod]
        public void Parse_inValidFormat_shouldReturnFalse()
        {
            const string data = "";
            var validator = new HmacValidator();

            var actual = validator.Parse(data);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Parse_validFormat_shouldReturnTrue()
        {
            var data = Guid.NewGuid().ToString();
            var validator = new HmacValidator();

            var actual = validator.Parse(data);

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        [ExpectedException(typeof (NotYetParsedException))]
        public void Validate_notYetParsed_shouldThrowException()
        {
            new HmacValidator().Validate();
        }

        [TestMethod]
        [ExpectedException(typeof (DependencyMissingException))]
        public void Validate_dependencyMissing_shouldThrowException()
        {
            var validator = new HmacValidator();
            validator.Parse(Guid.NewGuid().ToString());

            validator.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof (HmacInvalidArgumentException))]
        public void Validate_queryStringNotSet_shouldThrowException()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var secret = "secret";
            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(appId, serviceId))
                .Returns(new AppModel {Enabled = false, Secret = secret});
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var mockAppValidator = new Mock<IValidator>();
            mockAppValidator.Setup(x => x.GetValue()).Returns(appId);
            var validator = new HmacValidator
            {
                HmacService = new HmacSha256Service(),
                AppRepository = mockAppRepository.Object,
                ServiceValidator = mockServiceValidator.Object,
                AppValidator = mockAppValidator.Object
            };
            validator.Parse(data);

            validator.Validate();
        }

        [TestMethod]
        public void Validate_mismatchingSignature_shouldReturnFalse()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            const string secret = "secret";
            var service = new HmacSha256Service();
            var queryString = new NameValueCollection {{"data", Guid.NewGuid().ToString()}};
            queryString.Add("signature", service.CalculateHash(service.CreateSortedQueryString(queryString), secret) + "1");

            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(appId, serviceId))
                .Returns(new AppModel {Enabled = false, Secret = secret});
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var mockAppValidator = new Mock<IValidator>();
            mockAppValidator.Setup(x => x.GetValue()).Returns(appId);
            var validator = new HmacValidator
            {
                HmacService = service,
                AppRepository = mockAppRepository.Object,
                ServiceValidator = mockServiceValidator.Object,
                AppValidator = mockAppValidator.Object
            };
            validator.SetQueryString(queryString);
            validator.Parse(queryString.Get("signature"));

            var actual = validator.Validate();

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Validate_queryStringSetMatchingSignature_shouldReturnTrue()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            const string secret = "secret";
            var service = new HmacSha256Service();
            var queryString = new NameValueCollection {{"data", Guid.NewGuid().ToString()}};
            queryString.Add("signature", service.CalculateHash(service.CreateSortedQueryString(queryString), secret));

            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository.Setup(x => x.Get(appId, serviceId))
                .Returns(new AppModel {Enabled = false, Secret = secret});
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var mockAppValidator = new Mock<IValidator>();
            mockAppValidator.Setup(x => x.GetValue()).Returns(appId);
            var validator = new HmacValidator
            {
                HmacService = service,
                AppRepository = mockAppRepository.Object,
                ServiceValidator = mockServiceValidator.Object,
                AppValidator = mockAppValidator.Object
            };
            validator.SetQueryString(queryString);
            validator.Parse(queryString.Get("signature"));

            var actual = validator.Validate();

            Assert.AreEqual(true, actual);            
        }
    }
}
