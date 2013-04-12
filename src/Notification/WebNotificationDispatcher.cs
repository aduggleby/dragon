using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    class WebNotificationDispatcher : INotificationDispatcher<IWebNotifiable>
    {
        public void Dispatch(IWebNotifiable notifiable, INotification notification)
        {
            throw new System.NotImplementedException();
        }
    }

}
