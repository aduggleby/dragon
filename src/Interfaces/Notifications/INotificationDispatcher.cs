namespace Dragon.Interfaces.Notifications
{
    public interface INotificationDispatcher<in T> where T: INotifiable
    {
        void Dispatch(T notifiable, INotification notification);
    }
}
