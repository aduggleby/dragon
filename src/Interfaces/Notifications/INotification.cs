using System;
using System.Collections.Specialized;

namespace Dragon.Interfaces.Notifications
{
    public interface INotification
    {
        Guid ID { get;  }
        string TypeKey { set; }
        StringDictionary Parameter { set; }
    }
}
