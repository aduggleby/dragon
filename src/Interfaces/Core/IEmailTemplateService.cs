using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces.ActivityCenter;
using Dragon.Interfaces.Core;

namespace Dragon.Interfaces
{
    public interface IEmailTemplateService
    {
        ITemplateServiceResult Generate(IEnumerable<IActivity> activity, string[] subtypeOrder, INotifiable notifiable);
    }
}