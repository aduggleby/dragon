using System;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class WebNotifiable : IWebNotifiable
    {
        public Guid UserID { get; set; }
    }
}
