using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NotificationTest
{
    [TestClass]
    public class EmailNotificationDispatcherTest : EmailNotificationBase
    {
        [TestMethod]
        public void DispatchShouldInvokeEmailService()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            emailService.Verify(_ => _.Send(EmailAddress, It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail));
        }

        [TestMethod]
        public void DispatchShouldInvokeLocalizedDataSource()
        {
            var localizedDataSource = new Mock<ILocalizedDataSource>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                new Mock<IEmailService>().Object, new Mock<ITemplateService>().Object, localizedDataSource.Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            localizedDataSource.Verify(_ => _.GetContent(MessageType, LanguageCode));
        }

        [TestMethod]
        public void DispatchShouldInvokeTemplateService()
        {
            var templateService = new Mock<ITemplateService>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                new Mock<IEmailService>().Object, templateService.Object, CreateLocalizedDataSourceStub().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            templateService.Verify(_ => _.Parse(TemplateText, Parameter));
        }

        [TestMethod]
        public void DispatchShouldSendCorrectSubjectAndMessage()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                emailService.Object, CreateTemplateServiceStub().Object, CreateLocalizedDataSourceStub().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            emailService.Verify(_ => _.Send(EmailAddress, Subject, Body, UseHtmlEmail));
        }
    }
}
