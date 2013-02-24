using System;

namespace Dragon.Interfaces
{
    public interface ISessionStore
    {
        ISession Session { get; }
        Guid ConnectedUserID { get; set; }
    }
}
