using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;

namespace Dragon.Mail.Impl
{
    public class DefaultSmtpClient: ISmtpClient
    {
        private SmtpClient m_client = new SmtpClient();

        public SmtpClient Client
        {
            get { return m_client; }
        }

        public void Send(MailMessage mm)
        {
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
