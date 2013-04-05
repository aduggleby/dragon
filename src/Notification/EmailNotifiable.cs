using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    class EmailNotifiable : IEmailNotifiable
    {
        public string PrimaryEmailAddress { get; private set; }
        public bool UseHTMLEmail { get; private set; }
    }
}
