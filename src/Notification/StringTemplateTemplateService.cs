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
            parameter.ToList().ForEach(_ => template.Add(_.Key, _.Value));
            return template.Render();
        }
    }
}