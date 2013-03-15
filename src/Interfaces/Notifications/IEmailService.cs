using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces.Notifications
{
    public interface IEmailService
    {
        void Send(string email, string subject, string body, bool useHtmlEmail);
    }
}
