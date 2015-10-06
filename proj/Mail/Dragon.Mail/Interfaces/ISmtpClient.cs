using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Interfaces
{
    public interface ISmtpClient
    {
        void Send(MailMessage mm);

        void SetHost(string host);
        void SetPort(int port);
        void SetEnableSsl(bool enableSsl);
        void SetCredentials(string domain, string user, string password);
    }
}
