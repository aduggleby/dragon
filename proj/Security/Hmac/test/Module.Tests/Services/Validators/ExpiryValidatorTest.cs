using System;
using Dragon.Security.Hmac.Module.Services.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Module.Tests.Services.Validators
{
    [TestClass]
    public class ExpiryValidatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void GetValue_notYetParsed_shouldThrowException()
        {
            new ExpiryValidator().GetValue();
        }

        [TestMethod]
        public void GetValue_alreadyParsed_shouldReturnValue()
        {
            var data = DateTime.Now.Ticks;
            var validator = new ExpiryValidator();
            validator.Parse(data.ToString());

            var actual = (long)validator.GetValue();

            Assert.AreEqual(data, actual);
        }

        [TestMethod]
        public void Parse_inValidFormat_shouldReturnFalse()
        {
            const string data = "not-a-date";
            var validator = new ExpiryValidator();

            var actual = validator.Parse(data);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Parse_validFormat_shouldReturnTrue()
        {
            var data = DateTime.Now.Ticks;
            var validator = new ExpiryValidator();

            var actual = validator.Parse(data.ToString());

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotYetParsedException))]
        public void Validate_notYetParsed_shouldThrowException()
        {
            new ExpiryValidator().Validate();
        }

        [TestMethod]
        public void Validate_invalidValue_shouldReturnFalse()
        {
            var data = DateTime.Now.Subtract(TimeSpan.FromDays(2)).Ticks.ToString();
            var validator = new ExpiryValidator();
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(false, actual);
        }
 
        [TestMethod]
        public void Validate_validValue_shouldReturnTrue()
        {
            var data = DateTime.Now.Add(TimeSpan.FromDays(2)).Ticks.ToString();
            var validator = new ExpiryValidator();
            validator.Parse(data);

            var actual = validator.Validate();

            Assert.AreEqual(true, actual);
        }
    }
}
