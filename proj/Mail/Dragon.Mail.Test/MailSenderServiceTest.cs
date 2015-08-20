using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net.Mime;
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

    }
}
