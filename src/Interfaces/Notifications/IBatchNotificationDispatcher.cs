using System;

namespace Dragon.Interfaces.Notifications
{
    public interface IBatchNotificationDispatcher<T> where T: INotifiable
    {
        void Add(T notifiable, INotification notification);
        void Dispatch();
    }
}
