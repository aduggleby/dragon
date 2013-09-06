using System;

namespace Dragon.Notification
{
    public class WebNotificationException : Exception
    {
        public WebNotificationException(string message) : base(message)
        {
        }
    }
}
