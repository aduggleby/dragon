using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class EmailBatchNotificationDispatcher : IBatchNotificationDispatcher<IEmailNotifiable>
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        private readonly ILocalizedDataSource _localizedDataSource;
        private readonly Dictionary<IEmailNotifiable, List<INotification>> _dispatchQueue = new Dictionary<IEmailNotifiable, List<INotification>>(new EmailEqComparer());

        public class EmailEqComparer : EqualityComparer<IEmailNotifiable>
        {
            public override bool Equals(IEmailNotifiable x, IEmailNotifiable y)
            {
                return x.PrimaryEmailAddress == y.PrimaryEmailAddress;
            }

            public override int GetHashCode(IEmailNotifiable obj)
            {
                Debug.Assert(obj != null, "obj != null");
                return obj.PrimaryEmailAddress.GetHashCode();
            }
        }

        public EmailBatchNotificationDispatcher(IEmailService emailService, ITemplateService templateService, ILocalizedDataSource localizedDataSource)
        {
            _emailService = emailService;
            _templateService = templateService;
            _localizedDataSource = localizedDataSource;
        }

        public void Dispatch(IEmailNotifiable notifiable, INotification notification)
        {
            if (!_dispatchQueue.ContainsKey(notifiable))
            {
                _dispatchQueue.Add(notifiable, new List<INotification>());
            }
            _dispatchQueue[notifiable].Add(notification);
        }

        public void DispatchAll(IEmailNotifiable notifiable, String subject)
        {
            if (!_dispatchQueue.ContainsKey(notifiable) || _dispatchQueue[notifiable].Count < 1) return;

            var body = new StringBuilder();
            foreach (var notification in _dispatchQueue[notifiable])
            {
                var bodyTemplate = _localizedDataSource.GetContent(notification.TypeKey, notification.LanguageCode);
                body.Append(_templateService.Parse(bodyTemplate, notification.Parameter));
            }
            _emailService.Send(notifiable.PrimaryEmailAddress, subject, body.ToString(), notifiable.UseHTMLEmail);

            _dispatchQueue[notifiable].Clear();
        }
    }
}
