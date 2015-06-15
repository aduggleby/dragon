using System;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Security.Hmac.Module.Tests.Services.Validators
{
    [TestClass]
    public class UserValidatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void GetValue_notYetParsed_shouldThrowException()
        {
            new UserValidator().GetValue();
        }

        [TestMethod]
        public void GetValue_alreadyParsed_shouldReturnValue()
        {
            var data = Guid.NewGuid();
            var validator = new UserValidator();
            validator.Parse(data.ToString());

            var actual = (Guid)validator.GetValue();

            Assert.AreEqual(data, actual);
        }

        [TestMethod]
        public void Parse_inValidFormat_shouldReturnFalse()
        {
            var data = Guid.NewGuid() + "2";
            var validator = new UserValidator();

            var actual = validator.Parse(data);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Parse_validFormat_shouldReturnTrue()
        {
            var data = Guid.NewGuid().ToString();
            var validator = new UserValidator();

            var actual = validator.Parse(data);

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void Validate_notYetParsed_shouldThrowException()
        {
            new UserValidator().Validate();
        }

        [TestMethod]
        [ExpectedException(typeof (DependencyMissingException))]
        public void Validate_dependencyMissing_shouldThrowException()
        {
            var validator = new UserValidator();
            validator.Parse(Guid.NewGuid().ToString());

            validator.Validate();
        }

        [TestMethod]
        public void Validate_userIsDisabled_shouldReturnFalse()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(appId, serviceId)).Returns(new UserModel {Enabled = false});
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var mockAppValidator = new Mock<IValidator>();
            mockAppValidator.Setup(x => x.GetValue()).Returns(appId);
            var validator = new UserValidator
            {
                UserRepository = mockUserRepository.Object, 
                ServiceValidator = mockServiceValidator.Object, 
                AppValidator = mockAppValidator.Object
            };
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(false, actual);
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }

       [TestMethod]
        public void Validate_userIsEnabled_shouldReturnTrue()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(appId, serviceId)).Returns(new UserModel {Enabled = true});
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var mockAppValidator = new Mock<IValidator>();
            mockAppValidator.Setup(x => x.GetValue()).Returns(appId);
            var validator = new UserValidator
            {
                UserRepository = mockUserRepository.Object, 
                ServiceValidator = mockServiceValidator.Object, 
                AppValidator = mockAppValidator.Object
            };
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(true, actual);
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }
        
        [TestMethod]
        public void Validate_userDoesNotExist_shouldReturnTrue()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(appId, serviceId)).Returns((UserModel)null);
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var mockAppValidator = new Mock<IValidator>();
            mockAppValidator.Setup(x => x.GetValue()).Returns(appId);
            var validator = new UserValidator
            {
                UserRepository = mockUserRepository.Object, 
                ServiceValidator = mockServiceValidator.Object, 
                AppValidator = mockAppValidator.Object
            };
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(true, actual);
            mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Never);
        }        

        [TestMethod]
        public void OnSuccess_userDoesNotExist_shouldAddUser()
        {
            var appId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var data = appId.ToString();
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.Get(appId, serviceId)).Returns((UserModel)null);
            var mockServiceValidator = new Mock<IValidator>();
            mockServiceValidator.Setup(x => x.GetValue()).Returns(serviceId);
            var mockAppValidator = new Mock<IValidator>();
            mockAppValidator.Setup(x => x.GetValue()).Returns(appId);
            var validator = new UserValidator
            {
                UserRepository = mockUserRepository.Object, 
                ServiceValidator = mockServiceValidator.Object, 
                AppValidator = mockAppValidator.Object
            };
            validator.Parse(data);
            validator.Validate();

            validator.OnSuccess();

            mockUserRepository.Verify(x => x.Insert(It.Is<UserModel>(y => y.Enabled && y.AppId == appId && y.ServiceId == serviceId)));
        }
    }
}
