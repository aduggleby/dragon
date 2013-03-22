using System.Collections.Specialized;

namespace Dragon.Interfaces.Notifications
{
    // TODO: move to common package
    public interface ITemplateService
    {
        void Parse(string text, StringDictionary parameters);
    }
}
