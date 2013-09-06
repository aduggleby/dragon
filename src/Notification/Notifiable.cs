using System;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    class Notifiable : IEmailNotifiable, IWebNotifiable
    {
        public string PrimaryEmailAddress { get; set; }
        public bool UseHTMLEmail { get; set; }
        public Guid UserID { get; set; }
    }
}
