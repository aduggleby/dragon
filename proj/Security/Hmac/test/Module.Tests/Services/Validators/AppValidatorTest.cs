using System;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Security.Hmac.Module.Tests.Services.Validators
{
    [TestClass]
    public class AppValidatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void GetValue_notYetParsed_shouldThrowException()
        {
            new AppValidator().GetValue();
        }

        [TestMethod]
        public void GetValue_alreadyParsed_shouldReturnValue()
        {
            var data = Guid.NewGuid();
            var validator = new AppValidator();
            validator.Parse(data.ToString());

            var actual = (Guid)validator.GetValue();

            Assert.AreEqual(data, actual);
        }

        [TestMethod]
        public void Parse_inValidFormat_shouldReturnFalse()
        {
            var data = Guid.NewGuid() + "2";
            var validator = new AppValidator();

            var actual = validator.Parse(data);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Parse_validFormat_shouldReturnTrue()
        {
            var data = Guid.NewGuid().ToString();
            var validator = new AppValidator();

            var actual = validator.Parse(data);

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void Validate_notYetParsed_shouldThrowException()
        {
            new AppValidator().Validate();
        }

        [TestMethod]
        [ExpectedException(typeof (DependencyMissingException))]
        public void Validate_dependencyMissing_shouldThrowException()
        {
            var validator = new AppValidator();
            validator.Parse(Guid.NewGuid().ToString());

            validator.Validate();
        }

        [TestMethod]
        public void Validate_appIsDisabled_shouldReturnFalse()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var mockAppRepository = new Mock<IAppRepository>();
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var validator = new AppValidator { AppRepository = mockAppRepository.Object, ServiceValidator = mockServiceValidator.Object };
            mockAppRepository.Setup(x => x.Get(appId, serviceId)).Returns(new AppModel {Enabled = false});
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(false, actual);
        }


        [TestMethod]
        public void Validate_appDoesNotExist_shouldReturnFalse()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var mockAppRepository = new Mock<IAppRepository>();
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var validator = new AppValidator { AppRepository = mockAppRepository.Object, ServiceValidator = mockServiceValidator.Object };
            mockAppRepository.Setup(x => x.Get(appId, serviceId)).Returns((AppModel)null);
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(false, actual);
        }
 

 
        [TestMethod]
        public void Validate_appExistsAndIsEnabled_shouldReturnTrue()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var mockAppRepository = new Mock<IAppRepository>();
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var validator = new AppValidator { AppRepository = mockAppRepository.Object, ServiceValidator = mockServiceValidator.Object };
            mockAppRepository.Setup(x => x.Get(appId, serviceId)).Returns(new AppModel {Enabled = true});
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(true, actual);
        }
    }
}
