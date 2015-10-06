using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class MailSenderServiceTest
    {
        [TestMethod]
        public void MailMessage_is_correct_if_html_and_text_available()
        {
            // ARRANGE
            var queueMock = new Mock<IMailQueue>();
            queueMock.Setup(m => m.Enqueue(It.IsAny<Models.Mail>(), It.IsAny<object>()));

            var configMock = new Mock<IConfiguration>();

            var subject = new MailSenderService(queueMock.Object, configMock.Object);

            var renderedMail = new RenderedMail();
            renderedMail.Sender = new MailAddress("bob@example.org");
            renderedMail.Receiver = new MailAddress("bob@example.org");
            renderedMail.Subject = "foo";
            renderedMail.Body = "htmlbody";
            renderedMail.TextBody = "textbody";

            // ACT
            var mailMessage = subject.CreateMailMessage(renderedMail);

            // ASSERT
            var html = mailMessage.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType == MediaTypeNames.Text.Html);
            Assert.AreEqual("htmlbody", new StreamReader(html.ContentStream).ReadToEnd());

            var text = mailMessage.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType == MediaTypeNames.Text.Plain);
            Assert.AreEqual("textbody", new StreamReader(text.ContentStream).ReadToEnd());
        }


        [TestMethod]
        public void MailMessage_is_correct_if_only_html_available()
        {
            // ARRANGE
            var queueMock = new Mock<IMailQueue>();
            queueMock.Setup(m => m.Enqueue(It.IsAny<Models.Mail>(), It.IsAny<object>()));

            var configMock = new Mock<IConfiguration>();

            var subject = new MailSenderService(queueMock.Object, configMock.Object);

            var renderedMail = new RenderedMail();
            renderedMail.Sender = new MailAddress("bob@example.org");
            renderedMail.Receiver = new MailAddress("bob@example.org");
            renderedMail.Subject = "foo";
            renderedMail.Body = "htmlbody";
            renderedMail.TextBody = null;

            // ACT
            var mailMessage = subject.CreateMailMessage(renderedMail);

            // ASSERT
            var html = mailMessage.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType == MediaTypeNames.Text.Html);
            Assert.AreEqual("htmlbody", new StreamReader(html.ContentStream).ReadToEnd());

            var text = mailMessage.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType == MediaTypeNames.Text.Plain);
            // html was transferred to plain text
            Assert.AreEqual("htmlbody", new StreamReader(text.ContentStream).ReadToEnd().Trim());
        }

        [TestMethod]
        public void MailMessage_is_correct_if_only_text_available()
        {
            // ARRANGE
            var queueMock = new Mock<IMailQueue>();
            queueMock.Setup(m => m.Enqueue(It.IsAny<Models.Mail>(), It.IsAny<object>()));

            var configMock = new Mock<IConfiguration>();

            var subject = new MailSenderService(queueMock.Object, configMock.Object);

            var renderedMail = new RenderedMail();
            renderedMail.Sender = new MailAddress("bob@example.org");
            renderedMail.Receiver = new MailAddress("bob@example.org");
            renderedMail.Subject = "foo";
            renderedMail.Body = null;
            renderedMail.TextBody = "textbody";

            // ACT
            var mailMessage = subject.CreateMailMessage(renderedMail);

            // ASSERT
            var html = mailMessage.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType == MediaTypeNames.Text.Html);
            Assert.AreEqual(null, html);

            var text = mailMessage.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType == MediaTypeNames.Text.Plain);
            Assert.AreEqual("textbody", new StreamReader(text.ContentStream).ReadToEnd());
        }


        [TestMethod]
        public void Recipient_is_overridden_if_set()
        {
            // ARRANGE
            var queueMock = new Mock<IMailQueue>();
            queueMock.Setup(m => m.Enqueue(It.IsAny<Models.Mail>(), It.IsAny<object>()));

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(MailSenderService.APP_KEY_OVERRIDE_RECIPIENT))
                .Returns("override@example.org");

            var smtpMock = new Mock<ISmtpClient>();

            var subject = new MailSenderService(queueMock.Object, configMock.Object);

            var renderedMail = new RenderedMail();
            renderedMail.Sender = new MailAddress("bob@example.org");
            renderedMail.Receiver = new MailAddress("bob@example.org");
            renderedMail.Subject = "foo";
            renderedMail.Body = "htmlbody";
            renderedMail.TextBody = "textbody";

            // ACT
            var mailMessage = subject.ProcessInternal(renderedMail, smtpMock.Object);


            // ASSERT
            smtpMock.Verify(c => c.Send(
                It.Is<MailMessage>(m =>
                    m.To.Count == 1
                    && m.To.First().Address == "override@example.org")),
                Times.Once);
        }

        [TestMethod]
        public void Smtp_Host_IsOverwritten_ButNoOther_WhenSet()
        {
            // ARRANGE
            var queueMock = new Mock<IMailQueue>();
            queueMock.Setup(m => m.Enqueue(It.IsAny<Models.Mail>(), It.IsAny<object>()));

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(MailSenderService.APP_KEY_OVERRIDE_SMTP_HOST))
                .Returns("example.org");

            var smtpMock = new Mock<ISmtpClient>();

            var subject = new MailSenderService(queueMock.Object, configMock.Object);

            var renderedMail = new RenderedMail();
            renderedMail.Sender = new MailAddress("bob@example.org");
            renderedMail.Receiver = new MailAddress("bob@example.org");
            renderedMail.Subject = "foo";
            renderedMail.Body = "htmlbody";
            renderedMail.TextBody = "textbody";

            // ACT
            var mailMessage = subject.ProcessInternal(renderedMail, smtpMock.Object);

            // ASSERT
            smtpMock.Verify(c => c.SetHost("example.org"), Times.Once);
            smtpMock.Verify(c => c.SetPort(It.IsAny<int>()), Times.Never);
            smtpMock.Verify(c => c.SetCredentials(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            smtpMock.Verify(c => c.SetEnableSsl(It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void Smtp_Host_And_Port_IsOverwritten_ButNoOther_WhenSet()
        {
            // ARRANGE
            var queueMock = new Mock<IMailQueue>();
            queueMock.Setup(m => m.Enqueue(It.IsAny<Models.Mail>(), It.IsAny<object>()));

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(MailSenderService.APP_KEY_OVERRIDE_SMTP_HOST))
                .Returns("example.org");
            configMock.Setup(m => m.GetValue(MailSenderService.APP_KEY_OVERRIDE_SMTP_PORT))
                .Returns("1234");

            var smtpMock = new Mock<ISmtpClient>();

            var subject = new MailSenderService(queueMock.Object, configMock.Object);

            var renderedMail = new RenderedMail();
            renderedMail.Sender = new MailAddress("bob@example.org");
            renderedMail.Receiver = new MailAddress("bob@example.org");
            renderedMail.Subject = "foo";
            renderedMail.Body = "htmlbody";
            renderedMail.TextBody = "textbody";

            // ACT
            var mailMessage = subject.ProcessInternal(renderedMail, smtpMock.Object);

            // ASSERT
            smtpMock.Verify(c => c.SetHost("example.org"), Times.Once);
            smtpMock.Verify(c => c.SetPort(1234), Times.Once);
            smtpMock.Verify(c => c.SetCredentials(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            smtpMock.Verify(c => c.SetEnableSsl(It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void Smtp_All_Credentials_IsOverwritten_ButNoOther_WhenSet()
        {
            // ARRANGE
            var queueMock = new Mock<IMailQueue>();
            queueMock.Setup(m => m.Enqueue(It.IsAny<Models.Mail>(), It.IsAny<object>()));

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(MailSenderService.APP_KEY_OVERRIDE_SMTP_USER))
                .Returns("a");
            configMock.Setup(m => m.GetValue(MailSenderService.APP_KEY_OVERRIDE_SMTP_PASSWORD))
                .Returns("c");

            var smtpMock = new Mock<ISmtpClient>();

            var subject = new MailSenderService(queueMock.Object, configMock.Object);

            var renderedMail = new RenderedMail();
            renderedMail.Sender = new MailAddress("bob@example.org");
            renderedMail.Receiver = new MailAddress("bob@example.org");
            renderedMail.Subject = "foo";
            renderedMail.Body = "htmlbody";
            renderedMail.TextBody = "textbody";

            // ACT
            var mailMessage = subject.ProcessInternal(renderedMail, smtpMock.Object);

            // ASSERT
            smtpMock.Verify(c => c.SetHost("example.org"), Times.Never);
            smtpMock.Verify(c => c.SetPort(1234), Times.Never);
            smtpMock.Verify(c => c.SetCredentials(null, "a", "c"), Times.Once);
            smtpMock.Verify(c => c.SetEnableSsl(It.IsAny<bool>()), Times.Never);
        }
    }
}
