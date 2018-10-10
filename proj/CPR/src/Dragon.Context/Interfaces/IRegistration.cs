using System;

namespace Dragon.Context.Interfaces
{
    public interface IRegistration : IUser
    {
        Guid RegistrationID { get; }
        string Service { get;  }
        string Key { get;  }
        string Secret { get;  }
    }
}
