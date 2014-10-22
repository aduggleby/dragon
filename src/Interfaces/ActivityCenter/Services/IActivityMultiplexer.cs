﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.ActivityCenter
{
    public interface IActivityMultiplexer
    {
        IEnumerable<INotifiable> Multiplex(IActivity activity);
    }
}