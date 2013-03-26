using System;
using System.Collections.Specialized;

namespace Dragon.Interfaces.Notifications
{
    public interface INotification
    {
        Guid ID { get;  }
        string TypeKey { get; set; }
        StringDictionary Parameter { get; set; }
    }
}
