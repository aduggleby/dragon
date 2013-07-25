namespace Dragon.Interfaces.Notifications
{
    public interface IEmailNotifiable : INotifiable
    {
        string PrimaryEmailAddress { get; }
        bool UseHTMLEmail { get; }
    }
}
