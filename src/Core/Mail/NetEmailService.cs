using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
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
        public const string DragonMailEmailDisplayName = "Dragon.Mail.EmailFromDisplayName";

        
        public const string DragonMailEmailEmailOverride = "Dragon.Mail.EmailOverride";
        public const string DragonUseDefaultSMTP = "Dragon.Mail.UseDefaultSMTP";


        public IConfiguration Configuration { get; set; }
        private SmtpClient _client;

        public void Send(string to, string subject, string body, bool useHtmlEmail)
        {
            SendInternal(to, subject, body, useHtmlEmail, null);
        }

        public void Send(string to, string subject, string body, bool useHtmlEmail, Dictionary<string, byte[]> attachments)
        {
            SendInternal(to, subject, body, useHtmlEmail, attachments);
        }

        private void SendInternal(string to, string subject, string body, bool useHtmlEmail, Dictionary<string, byte[]> attachments)
        {
            Trace.WriteLine("SendInternal email to " + to + ": " + subject);

            var client = GetSmtpClient();

            var msg = new MailMessage();
            
            var overrideemail = Configuration.GetValue(DragonMailEmailEmailOverride, (string) null);

            if (string.IsNullOrWhiteSpace(overrideemail))
            {
                msg.To.Add(to);
            }
            else
            {
                msg.To.Add(overrideemail);
            }

            msg.From = new MailAddress(Configuration.GetValue(DragonMailEmailFrom, String.Empty), Configuration.GetValue(DragonMailEmailDisplayName, String.Empty));
            msg.Subject = subject;
            msg.IsBodyHtml = useHtmlEmail;
            msg.Body = body;

            Trace.WriteLine("Sending email to " + msg.To[0].Address + ": " + msg.Subject);

            var m_openedStreams = new List<Stream>();

            if (attachments != null)
            {

                foreach (var attachment in attachments)
                {
                    var ms = new MemoryStream(attachment.Value);
                    ms.Position = 0;
                    m_openedStreams.Add(ms);
                    msg.Attachments.Add(new Attachment(ms, attachment.Key));

                }
            }

            try
            {
                client.Send(msg); // TODO ASYNC
            }
            catch (Exception ex)
            {
                Trace.Fail("Failure sending email to " + msg.To[0].Address + ". Error: " + ex.Message);

                throw new Exception(
                    Configuration.GetValue<bool>(DragonUseDefaultSMTP, true) ? 
                    "Failure sending email with default configuration." :
                    string.Format("Failure sending email with configuration ({0}:{1}, {2}) \r\n{3}",
                    _client.Host, _client.Port, Configuration.GetValue(DragonMailSmtpUserKey, String.Empty),
                    ex.ToString()));
            }
            finally
            {
                m_openedStreams.ForEach(x => x.Dispose());
            }
        }

        protected SmtpClient GetSmtpClient()
        {
            return _client ??
                (_client = Configuration.GetValue<bool>(DragonUseDefaultSMTP, true) ? new SmtpClient() : new SmtpClient
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