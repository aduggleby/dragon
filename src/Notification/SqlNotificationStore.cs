using System;
using System.Collections.Generic;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class SqlNotificationStore : INotificationStore
    {
        public void AddNotification(out INotification notification)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<INotification> List { get; private set; }
    }
}
