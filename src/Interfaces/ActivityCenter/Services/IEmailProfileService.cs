using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.ActivityCenter
{
    public interface IEmailProfileService
    {
        IEnumerable<IActivity> AppendAndFlushIfNecessary(INotifiable notifiable, IActivity activity);
        IEmailProfile GetProfile(INotifiable notifiable);
    }
}
