using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Mail;
using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace NotificationTest
{
    [TestClass]
    public class NetEmailServiceTest
    {
        private const string SMTP_SERVER = "localhost";
        private const int SMTP_PORT = 25;
        private const string SUBJECT = "ohai";
        private const string BODY = "test abc content";
        private const string EMAIL_ADDRESS = "test@test.com";
        private const bool USE_HTML_EMAIL = false;

        [TestMethod]
        public void SendShouldInvokeSmtpClientsSend()
        {
            /*
            var netEmailService = new Mock<NetEmailService> { CallBase = true };
            var smtpClient = new Mock<SmtpClient>();
            netEmailService.Protected().Setup<SmtpClient>("GetSmtpClient").Returns(smtpClient.Object);

            netEmailService.Object.Send(EMAIL_ADDRESS, SUBJECT, BODY, USE_HTML_EMAIL);
            
            smtpClient.Verify(_ => _.Send(It.IsAny<String>(), EMAIL_ADDRESS, SUBJECT, BODY));
            */
            Assert.Fail(); // TODO
        }

        [TestMethod]
        public void GetSmtpClientShouldRetrieveConfigFromConfigurationBase()
        {
            Assert.Fail(); // TODO
        }
    }
}
