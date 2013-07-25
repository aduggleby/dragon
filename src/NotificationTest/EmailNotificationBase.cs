using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dragon.Interfaces.Notifications;
using Moq;

namespace NotificationTest
{
    public abstract class EmailNotificationBase
    {
        protected static readonly Dictionary<string, string> Parameter = new Dictionary<string, string> {{"name", "abc"}};
        protected const string TemplateText = "test {name} content.";
        protected const string Subject = "ohai";
        protected const string Body = "test abc content";
        protected const string EmailAddress = "test@test.com";
        protected const bool UseHtmlEmail = false;
        protected const string MessageType = "message_type";
        protected const string LanguageCode = "at";

        protected static Mock<IEmailNotifiable> CreateNotifiableStub()
        {
            var notifiable = new Mock<IEmailNotifiable>();
            notifiable.Setup(_ => _.PrimaryEmailAddress).Returns(EmailAddress);
            notifiable.Setup(_ => _.UseHTMLEmail).Returns(UseHtmlEmail);
            return notifiable;
        }

        protected static Mock<ILocalizedDataSource> CreateLocalizedDataSourceStub()
        {
            var localizedDataSource = new Mock<ILocalizedDataSource>();
            localizedDataSource.Setup(_ => _.GetContent(MessageType, LanguageCode)).Returns(TemplateText);
            return localizedDataSource;
        }

        protected static Mock<ITemplateService> CreateTemplateServiceStub()
        {
            var templateService = new Mock<ITemplateService>();
            templateService.Setup(_ => _.Parse(TemplateText, Parameter)).Returns(Body);
            return templateService;
        }

        protected static Mock<INotification> CreateNotificationStub()
        {
            var notification = new Mock<INotification>();
            notification.Setup(_ => _.TypeKey).Returns(MessageType);
            notification.Setup(_ => _.Parameter).Returns(Parameter);
            notification.Setup(_ => _.LanguageCode).Returns(LanguageCode);
            notification.Setup(_ => _.Subject).Returns(Subject);
            return notification;
        }
    }
}
