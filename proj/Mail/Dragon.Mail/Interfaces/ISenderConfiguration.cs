﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Interfaces
{
    public interface ISenderConfiguration
    {
        void Configure(Models.Mail mail);
    }
}