using System;

namespace Dragon.Interfaces.Notifications
{
    public interface IBatchNotificationDispatcher<T> : INotificationDispatcher<T> where T: INotifiable
    {
        void DispatchAll(T notifiable, String subject);
    }
}
