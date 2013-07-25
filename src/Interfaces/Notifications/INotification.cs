using System;
using System.Collections.Generic;

namespace Dragon.Interfaces.Notifications
{
    public interface INotification
    {
        Guid ID { get;  }
        string TypeKey { get; set; }
        string LanguageCode { get; set; }
        Dictionary<string, string> Parameter { get; set; }
        string Subject { get; set; }
    }
}
