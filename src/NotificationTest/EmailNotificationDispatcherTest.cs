using System.Collections.Specialized;
using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NotificationTest
{
    [TestClass]
    public class EmailNotificationDispatcherTest
    {
        [TestMethod]
        public void DispatchShouldInvokeEmailService()
        {
            const string emailAddress = "test@test.com";
            const bool useHtmlEmail = false;
            var emailService = new Mock<IEmailService>();
            var templateService = new Mock<ITemplateService>();
            var emailNotificationDispatcher = new EmailNotificationDispatcher(emailService.Object, templateService.Object);
            var notifiable = new Mock<IEmailNotifiable>();
            notifiable.Setup(_ => _.PrimaryEmailAddress).Returns(emailAddress);
            notifiable.Setup(_ => _.UseHTMLEmail).Returns(useHtmlEmail);
            var notification = new Mock<INotification>();
            notification.Setup(_ => _.TypeKey).Returns("type");
            notification.Setup(_ => _.Parameter).Returns(new StringDictionary());
            emailNotificationDispatcher.Dispatch(notifiable.Object, notification.Object);
            emailService.Verify(_ => _.Send(emailAddress, It.IsAny<string>(), It.IsAny<string>(), useHtmlEmail));
        }

        [TestMethod]
        public void DispatchShouldInvokeTemplateService()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void DispatchShouldSendCorrectSubjectAndMessage()
        {
            Assert.Fail();
        }

    }
}
