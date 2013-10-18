using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.ActivityCenter
{
    public interface IEmailProfile
    {
        string PrimaryEmailAddress{get;}
        bool WantsHtml{get;}
    }
}
