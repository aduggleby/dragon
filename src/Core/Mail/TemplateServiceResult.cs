using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces.Core;

namespace Dragon.Core.Mail
{
    public class TemplateServiceResult : ITemplateServiceResult
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Subtype { get; set; }
    }
}
