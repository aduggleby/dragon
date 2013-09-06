using System;

namespace Dragon.Interfaces.Notifications
{
    public interface IWebNotifiable : INotifiable
    {
        Guid UserID { get; }
    }
}
