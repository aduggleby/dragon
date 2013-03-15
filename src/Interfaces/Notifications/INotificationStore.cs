using Dragon.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.Notifications
{
    public interface INotificationStore
    {
        // TODO: refactor paramters to object?
        void AddNotification(Guid notificationId, string notificationTypeKey, StringDictionary parameters);
        IEnumerable<IDictionary<Guid, StringDictionary>> List { get; }
    }
}
