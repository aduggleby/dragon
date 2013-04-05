using System.Collections.Generic;
using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NotificationTest
{
    [TestClass]
    public class EmailNotificationDispatcherTest
    {
        private static readonly Dictionary<string, string> Parameter = new Dictionary<string, string> {{"name", "abc"}};
        private const string TEMPLATE_TEXT = "test {name} content.";
        private const string SUBJECT = "ohai";
        private const string BODY = "test abc content";
        private const string EMAIL_ADDRESS = "test@test.com";
        private const bool USE_HTML_EMAIL = false;
        private const string MESSAGE_TYPE = "message_type";
        private const string LANGUAGE_CODE = "at";

        [TestMethod]
        public void DispatchShouldInvokeEmailService()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            emailService.Verify(_ => _.Send(EMAIL_ADDRESS, It.IsAny<string>(), It.IsAny<string>(), USE_HTML_EMAIL));
        }

        [TestMethod]
        public void DispatchShouldInvokeLocalizedDataSource()
        {
            var localizedDataSource = new Mock<ILocalizedDataSource>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                new Mock<IEmailService>().Object, new Mock<ITemplateService>().Object, localizedDataSource.Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            localizedDataSource.Verify(_ => _.GetContent(MESSAGE_TYPE, LANGUAGE_CODE));
        }

        [TestMethod]
        public void DispatchShouldInvokeTemplateService()
        {
            var templateService = new Mock<ITemplateService>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                new Mock<IEmailService>().Object, templateService.Object, CreateLocalizedDataSourceStub().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            templateService.Verify(_ => _.Parse(TEMPLATE_TEXT, Parameter));
        }

        [TestMethod]
        public void DispatchShouldSendCorrectSubjectAndMessage()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(
                emailService.Object, CreateTemplateServiceStub().Object, CreateLocalizedDataSourceStub().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            emailService.Verify(_ => _.Send(EMAIL_ADDRESS, SUBJECT, BODY, USE_HTML_EMAIL));
        }

        private static Mock<ITemplateService> CreateTemplateServiceStub()
        {
            var templateService = new Mock<ITemplateService>();
            templateService.Setup(_ => _.Parse(TEMPLATE_TEXT, Parameter)).Returns(BODY);
            return templateService;
        }

        private static Mock<INotification> CreateNotificationStub()
        {
            var notification = new Mock<INotification>();
            notification.Setup(_ => _.TypeKey).Returns(MESSAGE_TYPE);
            notification.Setup(_ => _.Parameter).Returns(Parameter);
            notification.Setup(_ => _.LanguageCode).Returns(LANGUAGE_CODE);
            notification.Setup(_ => _.Subject).Returns(SUBJECT);
            return notification;
        }

        private static Mock<IEmailNotifiable> CreateNotifiableStub()
        {
            var notifiable = new Mock<IEmailNotifiable>();
            notifiable.Setup(_ => _.PrimaryEmailAddress).Returns(EMAIL_ADDRESS);
            notifiable.Setup(_ => _.UseHTMLEmail).Returns(USE_HTML_EMAIL);
            return notifiable;
        }

        private static Mock<ILocalizedDataSource> CreateLocalizedDataSourceStub()
        {
            var localizedDataSource = new Mock<ILocalizedDataSource>();
            localizedDataSource.Setup(_ => _.GetContent(MESSAGE_TYPE, LANGUAGE_CODE)).Returns(TEMPLATE_TEXT);
            return localizedDataSource;
        }

    }
}
