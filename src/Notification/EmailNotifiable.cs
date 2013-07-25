using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class EmailNotifiable : IEmailNotifiable
    {
        public string PrimaryEmailAddress { get; set; }
        public bool UseHTMLEmail { get; set; }
    }
}
