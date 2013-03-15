namespace Dragon.Interfaces.Notifications
{
    public interface IEmailService
    {
        void Send(string email, string subject, string body, bool useHtmlEmail);
    }
}
