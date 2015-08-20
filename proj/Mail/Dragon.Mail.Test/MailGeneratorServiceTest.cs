using System;
using System.Dynamic;
using System.Linq.Expressions;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class MailGeneratorServiceTest
    {
        [TestMethod]
        public void Basic_AsyncOff()
        {
            // ARRANGE
            var senderMock = new Mock<IMailSenderService>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_ADDRESS))
                .Returns("me@example.org");
            configMock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_NAME))
                .Returns("John Doe");

            dynamic url1 = new ExpandoObject();
            url1.Var1 = "Foo1";
            url1.Var2 = "Bar2";

            dynamic url2 = new ExpandoObject();
            dynamic url2sub = new ExpandoObject();
            url2.Sub1 = url2sub;
            url2sub.Var3 = "Foo3";
            url2sub.Var4 = "Bar4";

            dynamic user = new ExpandoObject();
            user.email = "tina@example.org";
            user.fullname = "Tina Test";
            user.fname = "Tina";

            var datastoreMock = new Mock<IDataStore>();
            datastoreMock.Setup(m => m.Get(new Uri("http://example.org/url1"))).Returns((object)url1);
            datastoreMock.Setup(m => m.Get(new Uri("http://example.org/url2"))).Returns((object)url2);
            datastoreMock.Setup(m => m.Get(new Uri("http://example.org/user"))).Returns((object)user);

            var queueMock = new Mock<IMailQueue>();

            var template = new Template();
            template.Key = "key1";
            template.Content.Subject = "{{user.fullname}} today";
            template.Content.Body = "Hello {{user.fullname}} <br/> {{two.Sub1.Var3}} {{one.Var2}}";
            template.Content.SummaryBody = "H {{user.fname}}";

            dynamic receiver = new Uri("http://example.org/user");

            dynamic data = new ExpandoObject();
            data.one = new Uri("http://example.org/url1");
            data.two = new Uri("http://example.org/url2");
            data.user = new Uri("http://example.org/user");

            var mailService = new MailGeneratorService(
                queueMock.Object,
                datastoreMock.Object,
                configuration: configMock.Object,
                mailSenderService: senderMock.Object);

            mailService.Register(template);

            // ACT
            mailService.Send(receiver, "key1", data);

            // ASSERT
            Expression<Func<Models.Mail, bool>> mailCheck = mail =>
                mail.Sender.Address == "me@example.org" &&
                       mail.Sender.DisplayName == "John Doe" &&
                       mail.Receiver.Address == "tina@example.org" &&
                       mail.Receiver.DisplayName == "Tina Test" &&
                       mail.Subject == "Tina Test today" &&
                       mail.Body == "Hello Tina Test <br/> Foo3 Bar2" &&
                       mail.SummaryBody == "H Tina";

            queueMock.Verify(m => m.Enqueue(It.Is<Models.Mail>(mailCheck), It.IsAny<object>()));
            senderMock.Verify(m => m.ProcessNext());
        }


        [TestMethod]
        public void Basic_AsyncOn()
        {
            // ARRANGE
            var senderMock = new Mock<IMailSenderService>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_ADDRESS))
                .Returns("me@example.org");
            configMock.Setup(m => m.GetValue(DefaultSenderConfiguration.APP_KEY_FROM_NAME))
                .Returns("John Doe");
            configMock.Setup(m => m.GetValue(MailGeneratorService.APP_KEY_ASYNCACTIVE))
                .Returns("1");

            dynamic url1 = new ExpandoObject();
            url1.Var1 = "Foo1";
            url1.Var2 = "Bar2";

            dynamic url2 = new ExpandoObject();
            dynamic url2sub = new ExpandoObject();
            url2.Sub1 = url2sub;
            url2sub.Var3 = "Foo3";
            url2sub.Var4 = "Bar4";

            dynamic user = new ExpandoObject();
            user.email = "tina@example.org";
            user.fullname = "Tina Test";
            user.fname = "Tina";

            var datastoreMock = new Mock<IDataStore>();
            datastoreMock.Setup(m => m.Get(new Uri("http://example.org/url1"))).Returns((object)url1);
            datastoreMock.Setup(m => m.Get(new Uri("http://example.org/url2"))).Returns((object)url2);
            datastoreMock.Setup(m => m.Get(new Uri("http://example.org/user"))).Returns((object)user);

            var queueMock = new Mock<IMailQueue>();

            var template = new Template();
            template.Key = "key1";
            template.Content.Body = "Hello {{user.fullname}} <br/> {{two.Sub1.Var3}} {{one.Var2}}";
            template.Content.SummaryBody = "H {{user.fname}}";
            template.Content.SummaryFooter = "B {{user.fname}}";
            template.Content.SummaryHeader = "C {{user.fname}}";
            template.Content.SummarySubject = "D {{user.fname}}";

            dynamic receiver = new Uri("http://example.org/user");

            dynamic data = new ExpandoObject();
            data.one = new Uri("http://example.org/url1");
            data.two = new Uri("http://example.org/url2");
            data.user = new Uri("http://example.org/user");

            var mailService = new MailGeneratorService(
                queueMock.Object,
                datastoreMock.Object,
                configuration: configMock.Object,
                mailSenderService: senderMock.Object);

            mailService.Register(template);

            // ACT
            mailService.Send(receiver, "key1", data);

            // ASSERT
            Expression<Func<Models.Mail, bool>> mailCheck = mail =>
                mail.Sender.Address == "me@example.org" &&
                       mail.Sender.DisplayName == "John Doe" &&
                       mail.Receiver.Address == "tina@example.org" &&
                       mail.Receiver.DisplayName == "Tina Test" &&
                       mail.Body == "Hello Tina Test <br/> Foo3 Bar2" &&
                       mail.SummaryBody == "H Tina" &&
                       mail.SummaryFooter == "B Tina" &&
                       mail.SummaryHeader == "C Tina" &&
                       mail.SummarySubject == "D Tina";

            queueMock.Verify(m => m.Enqueue(It.Is<Models.Mail>(mailCheck), It.IsAny<object>()));
            senderMock.Verify(m => m.ProcessNext(), Times.Never);
        }
    }
}
