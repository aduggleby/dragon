using System;

namespace Dragon.Interfaces
{
    public interface IRegistration : IUser
    {
        Guid RegistrationID { get; }
        string Service { get;  }
        string Key { get;  }
        string Secret { get;  }
    }
}
