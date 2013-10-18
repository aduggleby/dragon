using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.ActivityCenter
{
    public interface INotificationDispatcher
    {
        void Notify(IActivity activity, INotifiable notifiable);
    }
}
