using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.Notifications
{
    // TODO: not sure if email is sufficient, what about further dispatchers?
    public interface INotifiable
    {
        Guid UserID { get; }
        string PrimaryEmailAddress { get; }
        bool UseHTMLEmail { get; }
    }
}
