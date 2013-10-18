using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.ActivityCenter
{
    public interface IActivityService
    {
        IActivity Get(IActivity template);
        void Save(IActivity activity);
        void SaveNotification(IActivity activity, INotifiable notifiable);
    }

}
