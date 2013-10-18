using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.ActivityCenter
{
    public interface INotifiable
    {
        CultureInfo PrimaryCulture { get; }
    }
}
