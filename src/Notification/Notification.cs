using System;
using System.Collections.Generic;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class Notification : INotification
    {
        public Guid ID { get; private set; }
        public string TypeKey { get; set; }
        public string LanguageCode { get; set; }
        public Dictionary<string, string> Parameter { get; set; }
        public string Subject { get; set; }
    }
}
