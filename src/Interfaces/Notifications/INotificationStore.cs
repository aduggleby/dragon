using System.Collections.Generic;

namespace Dragon.Interfaces.Notifications
{
    public interface INotificationStore
    {
        void AddNotification(INotification notification);
        IEnumerable<INotification> GetAllNotifications();
    }
}
