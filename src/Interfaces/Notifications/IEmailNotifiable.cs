using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.Notifications
{
    public interface IEmailNotifiable : INotifiable
    {
        string PrimaryEmailAddress { get; }
        bool UseHTMLEmail { get; }
    }
}
