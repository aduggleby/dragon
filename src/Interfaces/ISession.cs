using System;

namespace Dragon.Interfaces
{
    public interface ISession
    {
        Guid ID{get;}
        string IPAddress { get; }
        string ForwardedForAddress { get; }
    }
}
