using System.Collections.Generic;

namespace Dragon.Interfaces.Notifications
{
    public interface INotificationStore
    {
        void AddNotification(out INotification notification);
        IEnumerable<INotification> List { get; }
    }
}
