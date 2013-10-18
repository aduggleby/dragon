using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.ActivityCenter
{
    public interface IEmailTemplate
    {
        string Subject { get; set; }
        string Body { get; set; }
        string HtmlBody { get; set; }
        bool SupportsHtml { get; set; }
    }
}
