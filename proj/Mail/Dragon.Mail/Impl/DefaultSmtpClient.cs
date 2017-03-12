using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;
using Common.Logging;

namespace Dragon.Mail.Impl
{
    public class DefaultSmtpClient : ISmtpClient
    {
        private Action<MailMessage> m_loggingAction = null;
        private SmtpClient m_client = new SmtpClient();

        public SmtpClient Client
        {
            get { return m_client; }
        }

        private static ILog s_log = LogManager.GetLogger<DefaultSmtpClient>();

        public DefaultSmtpClient(Action<MailMessage> loggingAction = null)
        {
            m_loggingAction = loggingAction;
        }

        public virtual void Send(MailMessage mm)
        {
            if (m_loggingAction != null)
            {
                try
                {
                    s_log.Trace(string.Format("Sending from {0} to {1} with subject {2}. Body length: {3}",
                        mm.From.ToString(),
                        string.Join(",", mm.To.Select(x => x.ToString())),
                        mm.Subject,
                        (mm.Body ?? string.Empty).Length
                        ));

                    m_loggingAction(mm);
                }
                catch (Exception ex)
                {
                    s_log.Error("Logging action failed.", ex);
                }
            }
            m_client.Send(mm);
        }

        public void SetHost(string host)
        {
            Client.Host = host;
        }

        public void SetPort(int port)
        {
            Client.Port = port;
        }

        public void SetEnableSsl(bool enableSsl)
        {
            Client.EnableSsl = enableSsl;
        }

        public void SetCredentials(string domain, string user, string password)
        {
            Client.Credentials = new NetworkCredential(user, password, domain);
        }
    }
}
