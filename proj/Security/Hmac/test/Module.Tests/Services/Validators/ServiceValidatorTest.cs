using System;
using Dragon.Security.Hmac.Module.Services.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Module.Tests.Services.Validators
{
    [TestClass]
    public class ServiceValidatorTest
    {
        private static readonly Guid ServiceId = Guid.NewGuid();

        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void GetValue_notYetParsed_shouldThrowException()
        {
            new ServiceValidator(ServiceId.ToString()).GetValue();
        }

        [TestMethod]
        public void GetValue_alreadyParsed_shouldReturnValue()
        {
            var data = Guid.NewGuid();
            var validator = new ServiceValidator(ServiceId.ToString());
            validator.Parse(data.ToString());

            var actual = (Guid)validator.GetValue();

            Assert.AreEqual(data, actual);
        }

        [TestMethod]
        public void Parse_inValidFormat_shouldReturnFalse()
        {
            var data = Guid.NewGuid() + "2";
            var validator = new ServiceValidator(ServiceId.ToString());

            var actual = validator.Parse(data);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Parse_validFormat_shouldReturnTrue()
        {
            var data = Guid.NewGuid().ToString();
            var validator = new ServiceValidator(ServiceId.ToString());

            var actual = validator.Parse(data);

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void Validate_notYetParsed_shouldThrowException()
        {
            new ServiceValidator(ServiceId.ToString()).Validate();
        }

        [TestMethod]
        public void Validate_mismatchingServiceId_shouldReturnFalse()
        {
            var validator = new ServiceValidator(ServiceId.ToString());
            validator.Parse(Guid.NewGuid().ToString());

            var actual = validator.Validate();

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Validate_matchingServiceId_shouldReturnTrue()
        {
            var validator = new ServiceValidator(ServiceId.ToString());
            validator.Parse(ServiceId.ToString());

            var actual = validator.Validate();

            Assert.AreEqual(true, actual);
        }
    }
}
