using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    class EmailNotificationDispatcher : INotificationDispatcher<IEmailNotifiable>
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;

        public EmailNotificationDispatcher(IEmailService emailService, ITemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public void Dispatch(IEmailNotifiable notifiable, INotification notification)
        {
            throw new System.NotImplementedException();
        }
    }
}
