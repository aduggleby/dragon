using System.Collections.Generic;

namespace Dragon.Interfaces.Core
{
    public interface IEmailService
    {
        void Send(string to, string subject, string body, bool useHtmlEmail);
        void Send(string to, string subject, string body, bool useHtmlEmail, Dictionary<string, byte[]> attachments);
    }
}
