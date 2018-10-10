using System;

namespace Dragon.Context.Interfaces
{
    public interface ISessionStore
    {
        ISession Session { get; }
        Guid ConnectedUserID { get; set; }
    }
}
