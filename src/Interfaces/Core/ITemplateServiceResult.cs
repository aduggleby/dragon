using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.Core
{
    public interface ITemplateServiceResult
    {
        string Subject { get; }
        string Body { get; }
        string Subtype { get; }
    }
}
