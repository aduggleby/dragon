using System;

namespace Dragon.Context.Interfaces
{
    public interface ISession
    {
        Guid ID {get;}
        string IPAddress { get; }
        string ForwardedForAddress { get; }
    }
}
