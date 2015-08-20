using System;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class DefaultSenderConfigurationTest
    {
        [TestMethod]
        public void WorksWithAddressOnly()
        {
            // ARRANGE
            var mock = new Mock<IConfiguration>();
            mock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_ADDRESS))
                .Returns("me@example.org");
            mock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_NAME))
                .Returns((string)null);

            var subject = new DefaultSenderConfiguration(mock.Object);
            var mail = new Models.Mail();

            // ACT
            subject.Configure(mail);

            // ASSERT
            Assert.AreEqual("me@example.org", mail.Sender.Address);
            Assert.AreEqual("", mail.Sender.DisplayName);

        }

        [TestMethod]
        public void WorksWithBoth()
        {
            // ARRANGE
            var mock = new Mock<IConfiguration>();
            mock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_ADDRESS))
                .Returns("me@example.org");
            mock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_NAME))
                .Returns("John Doe");

            var subject = new DefaultSenderConfiguration(mock.Object);
            var mail = new Models.Mail();

            // ACT
            subject.Configure(mail);

            // ASSERT
            Assert.AreEqual("me@example.org", mail.Sender.Address);
            Assert.AreEqual("John Doe", mail.Sender.DisplayName);

        }
    }
}
