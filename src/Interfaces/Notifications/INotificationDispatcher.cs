namespace Dragon.Interfaces.Notifications
{
    public interface INotificationDispatcher
    {
        void Dispatch(INotifiable notifiable, INotification notification);
    }
}
