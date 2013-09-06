using Dragon.Interfaces.Notifications;
using Dragon.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NotificationTest
{
    [TestClass]
    public class EmailBatchNotificationDispatcherTest : EmailNotificationBase
    {
        [TestMethod]
        public void DispatchShouldNotSendMailInstantly()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailBatchNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);

            emailService.Verify(x => x.Send(EmailAddress, It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail), Times.Never());
        }

        [TestMethod]
        public void DispatchShouldStoreNotificationForNextDispatchAll()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailBatchNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);
            emailNotificationDispatcher.DispatchAll(CreateNotifiableStub().Object, Subject);
            emailService.Verify(x => x.Send(EmailAddress, It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail), Times.Exactly(1));
        }

        [TestMethod]
        public void DispatchAllShouldSendOneMessageForAllDispatchedMessagesUntilLastDispatchAll()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailBatchNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);
            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);
            emailNotificationDispatcher.DispatchAll(CreateNotifiableStub().Object, Subject);
            emailService.Verify(x => x.Send(EmailAddress, It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail), Times.Exactly(1));
        }

        [TestMethod]
        public void DispatchAllShouldNotInvokeEmailServiceIfNoEntryForGivenNotifiableExists()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailBatchNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            emailNotificationDispatcher.DispatchAll(CreateNotifiableStub().Object, Subject);
            emailService.Verify(x => x.Send(EmailAddress, It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail), Times.Exactly(0));
        }

        [TestMethod]
        public void DispatchAllShouldSendNotificationsOnlyOnceOnSubsequentCalls()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailBatchNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);
            emailNotificationDispatcher.Dispatch(CreateNotifiableStub().Object, CreateNotificationStub().Object);
            emailNotificationDispatcher.DispatchAll(CreateNotifiableStub().Object, Subject);
            emailNotificationDispatcher.DispatchAll(CreateNotifiableStub().Object, Subject);
            emailService.Verify(x => x.Send(EmailAddress, It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail), Times.Exactly(1));
        }

        [TestMethod]
        public void DispatchAllShouldSendNotificationOnlyForSpecifiedNotifiable()
        {
            var emailService = new Mock<IEmailService>();
            var emailNotificationDispatcher = new EmailBatchNotificationDispatcher(
                emailService.Object, new Mock<ITemplateService>().Object, new Mock<ILocalizedDataSource>().Object);

            var notifiable1 = CreateNotifiableStub().Object;
            var notifiable = new Mock<IEmailNotifiable>();
            notifiable.Setup(x => x.PrimaryEmailAddress).Returns(EmailAddress + "2");
            notifiable.Setup(x => x.UseHTMLEmail).Returns(UseHtmlEmail);
            var notifiable2 = notifiable.Object;
            emailNotificationDispatcher.Dispatch(notifiable1, CreateNotificationStub().Object);
            emailNotificationDispatcher.Dispatch(notifiable2, CreateNotificationStub().Object);
            emailNotificationDispatcher.DispatchAll(notifiable1, Subject);
            emailNotificationDispatcher.DispatchAll(notifiable2, Subject);
            emailService.Verify(x => x.Send(EmailAddress, It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail), Times.Exactly(1));
            emailService.Verify(x => x.Send(EmailAddress + "2", It.IsAny<string>(), It.IsAny<string>(), UseHtmlEmail), Times.Exactly(1));
        }

    }
}
