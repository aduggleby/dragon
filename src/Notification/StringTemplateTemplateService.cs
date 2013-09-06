using System.Collections.Generic;
using Antlr4.StringTemplate;
using Dragon.Interfaces.Notifications;
using System.Linq;

namespace Dragon.Notification
{
    public class StringTemplateTemplateService : ITemplateService
    {
        public string Parse(string text, Dictionary<string, string> parameter)
        {
            var template = new Template(text);
            parameter.ToList().ForEach(entry => template.Add(entry.Key, entry.Value));
            return template.Render();
        }
    }
}