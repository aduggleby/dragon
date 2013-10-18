using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Dragon.Interfaces;
using Dragon.Interfaces.Core;

namespace Dragon.Notification
{
    public class NetEmailService : IEmailService
    {
        public const string DragonMailSmtpServerKey = "Dragon.Mail.SmtpServer";
        public const string DragonMailSmtpPortKey = "Dragon.Mail.SmtpPort";
        public const string DragonMailSmtpUserKey = "Dragon.Mail.SmtpUser";
        public const string DragonMailSmtpPasswordKey = "Dragon.Mail.SmtpPassword";
        public const string DragonMailEmailFrom = "Dragon.Mail.EmailFrom";
        public const string DragonMailEmailEmailOverride = "Dragon.Mail.EmailOverride";


        public IConfiguration Configuration { get; set; }
        private SmtpClient _client;

        public void Send(string to, string subject, string body, bool useHtmlEmail)
        {

            var client = GetSmtpClient();

            var msg = new MailMessage();
            msg.To.Add(Configuration.GetValue(DragonMailEmailEmailOverride, (string)null) ?? to);
            msg.From = new MailAddress(Configuration.GetValue(DragonMailEmailFrom, String.Empty));
            msg.Subject = subject;
            msg.IsBodyHtml = useHtmlEmail;
            msg.Body = body;


            client.Send(msg); // TODO ASYNC
        }

        protected SmtpClient GetSmtpClient()
        {
            return _client ??
                (_client = new SmtpClient
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