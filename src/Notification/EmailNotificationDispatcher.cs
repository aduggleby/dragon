using System;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class EmailNotificationDispatcher : INotificationDispatcher<IEmailNotifiable>
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        private readonly ILocalizedDataSource _dataSource;

        public EmailNotificationDispatcher(IEmailService emailService, ITemplateService templateService, ILocalizedDataSource dataSource)
        {
            _emailService = emailService;
            _templateService = templateService;
            _dataSource = dataSource;
        }

        public void Dispatch(IEmailNotifiable notifiable, INotification notification)
        {
            var bodyTemplate = _dataSource.GetContent(notification.TypeKey, notification.LanguageCode);
            var body = _templateService.Parse(bodyTemplate, notification.Parameter);
            _emailService.Send(notifiable.PrimaryEmailAddress, notification.Subject, body, notifiable.UseHTMLEmail);
        }
    }
}
