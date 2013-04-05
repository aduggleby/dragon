using System;
using System.Net;
using System.Net.Mail;
using Dragon.Interfaces;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class NetEmailService : IEmailService
    {
        public IConfiguration Configuration { get; set; }
        private SmtpClient _client;

        public void Send(string email, string subject, string body, bool useHtmlEmail)
        {
            var client = GetSmtpClient();
            client.Send(Configuration.GetValue("Dragon.Mail.DefaultEmailFrom", ""), email, subject, body); // TODO: where to get defaults?
        }

        protected SmtpClient GetSmtpClient()
        {
            return _client ??
                (_client = new SmtpClient // TODO: where to get the defaults? should not be hardcoded, right?
                {
                    Host = Configuration.GetValue("Dragon.Mail.SmtpServer", ""),
                    Port = Convert.ToInt32(Configuration.GetValue("Dragon.Mail.SmtpPort", "")),
                    Credentials = new NetworkCredential(
                        Configuration.GetValue("Dragon.Mail.SmtpUser", ""),
                        Configuration.GetValue("Dragon.Mail.SmtpPassword", "")
                        )
                });
        }
    }
}