using System;
using System.Net;
using System.Net.Mail;
using Dragon.Interfaces;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class NetEmailService : IEmailService
    {
        public const string DragonMailSmtpServerKey = "Dragon.Mail.SmtpServer";
        public const string DragonMailSmtpPortKey = "Dragon.Mail.SmtpPort";
        public const string DragonMailSmtpUserKey = "Dragon.Mail.SmtpUser";
        public const string DragonMailSmtpPasswordKey = "Dragon.Mail.SmtpPassword";
        public const string DragonMailEmailFrom = "Dragon.Mail.EmailFrom";

        public IConfiguration Configuration { get; set; }
        private SmtpClient _client;

        public void Send(string email, string subject, string body, bool useHtmlEmail)
        {
            var client = GetSmtpClient();
            client.Send(Configuration.GetValue(DragonMailEmailFrom, String.Empty), email, subject, body); // TODO: where to get defaults?
        }

        protected SmtpClient GetSmtpClient()
        {
            return _client ??
                (_client = new SmtpClient // TODO: where to get the defaults? should not be hardcoded, right?
                {
                    Host = Configuration.GetValue(DragonMailSmtpServerKey, String.Empty),
                    Port = Convert.ToInt32(Configuration.GetValue(DragonMailSmtpPortKey, String.Empty)),
                    Credentials = new NetworkCredential(
                        Configuration.GetValue(DragonMailSmtpUserKey, String.Empty),
                        Configuration.GetValue(DragonMailSmtpPasswordKey, String.Empty)
                        )
                });
        }
    }
}