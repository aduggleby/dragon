namespace Dragon.Interfaces.Notifications
{
    // TODO: move to common package
    public interface IEmailService
    {
        void Send(string email, string subject, string body, bool useHtmlEmail);
    }
}
