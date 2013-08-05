using System;
using System.Collections.Generic;

namespace Dragon.Interfaces.Notifications
{
    public interface INotificationStore
    {
        void Add(Guid userID, INotification notification);
        IEnumerable<INotification> GetAll(Guid userID);
        void SetDispatched(Guid notificationID);
        IEnumerable<INotification> GetAllUndispatched(Guid userID);
        void SetAllDispatched(Guid userID);
    }
}
