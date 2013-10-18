namespace Dragon.Interfaces.Core
{
    public interface IEmailService
    {
        void Send(string to, string subject, string body, bool useHtmlEmail);
    }
}
