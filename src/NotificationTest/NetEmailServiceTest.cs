using System;
using System.Net.Mail;
using Dragon.Interfaces;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NotificationTest
{
    [TestClass]
    public class NetEmailServiceTest
    {
        private const string USERNAME = "user";
        private const string SMTP_SERVER = "localhost";
        private const int SMTP_PORT = 25;
        private const string SUBJECT = "ohai";
        private const string BODY = "test abc content";
        private const string EMAIL_ADDRESS = "test@test.com";
        private const bool USE_HTML_EMAIL = false;
        private const string PASSWORD = "password";
        private const string MAIL_FROM = "user@test.com";

        public class NetEmailServiceWrapper : NetEmailService
        {
            public virtual new SmtpClient GetSmtpClient()
            {
                return base.GetSmtpClient();
            }
        }

        class ExtendedNetEmailServiceWrapper : NetEmailService
        {
            public SmtpClient SmtpClient { get; set; }

            protected override SmtpClient GetSmtpClient()
            {
                return SmtpClient;
            }
        }


        [TestMethod]
        [Ignore] // mocking of non-virtual members is not possible with moq =(
        public void SendShouldInvokeSmtpClientsSend()
        {
            var smtpClient = new Mock<SmtpClient>();
            smtpClient.Setup(_ =>_.Send(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()));
            var netEmailService = new ExtendedNetEmailServiceWrapper
            {
                Configuration = CreateConfigurationMock().Object,
                SmtpClient = smtpClient.Object
            };
            netEmailService.Send(EMAIL_ADDRESS, SUBJECT, BODY, USE_HTML_EMAIL);
            
            smtpClient.Verify(_ => _.Send(It.IsAny<String>(), EMAIL_ADDRESS, SUBJECT, BODY));
        }

        [TestMethod]
        public void GetSmtpClientShouldRetrieveConfigFromConfigurationBase()
        {
            var netEmailService = new NetEmailServiceWrapper {Configuration = CreateConfigurationMock().Object};

            var actual = netEmailService.GetSmtpClient();
            
            Assert.AreEqual(SMTP_SERVER, actual.Host);
            Assert.AreEqual(SMTP_PORT, actual.Port);
            Assert.AreEqual(USERNAME,  actual.Credentials.GetCredential(SMTP_SERVER, SMTP_PORT, "basic").UserName);
            Assert.AreEqual(PASSWORD,  actual.Credentials.GetCredential(SMTP_SERVER, SMTP_PORT, "basic").Password);
        }

        private static Mock<IConfiguration> CreateConfigurationMock()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(_ => _.GetValue(NetEmailService.DragonMailSmtpServerKey, String.Empty)).Returns(SMTP_SERVER);
            configuration.Setup(_ => _.GetValue(NetEmailService.DragonMailSmtpPortKey, String.Empty))
                .Returns(SMTP_PORT.ToString);
            configuration.Setup(_ => _.GetValue(NetEmailService.DragonMailSmtpUserKey, String.Empty)).Returns(USERNAME);
            configuration.Setup(_ => _.GetValue(NetEmailService.DragonMailSmtpPasswordKey, String.Empty)).Returns(PASSWORD);
            configuration.Setup(_ => _.GetValue(NetEmailService.DragonMailEmailFrom, String.Empty)).Returns(MAIL_FROM);
            return configuration;
        }
    }
}
