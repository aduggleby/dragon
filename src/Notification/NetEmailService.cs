using System.Net.Mail;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class NetEmailService : IEmailService
    {
        public void Send(string email, string subject, string body, bool useHtmlEmail)
        {
            //var client = new SmtpClient {Host = "mail.myisp.net"};
            //client.Send("from@adomain.com", "to@adomain.com", "subject", "body");
            throw new System.NotImplementedException();
        }
    }
}
