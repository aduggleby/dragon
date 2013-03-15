using System.Collections.Specialized;

namespace Dragon.Interfaces.Notifications
{
    public interface INotificationDispatcher
    {
        /// <param name="notifiable">The object to be notified.</param>
        /// <param name="notificationTypeKey">References a resource pack or template.</param>
        /// <param name="parameters">The arguments of the notification.</param>
        void Dispatch(INotifiable notifiable, string notificationTypeKey, StringDictionary parameters);
    }
}
