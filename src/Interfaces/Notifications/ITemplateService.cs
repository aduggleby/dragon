using System.Collections.Generic;

namespace Dragon.Interfaces.Notifications
{
    // TODO: move to common package
    public interface ITemplateService
    {
        void Parse(string text, Dictionary<string, string> parameters);
    }
}
