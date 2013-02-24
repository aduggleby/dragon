using System;

namespace Dragon.Interfaces
{
    public interface IUserStore
    {
        IUser User { get; }

        bool TryLogin(string service, string key, Func<string,bool> secretVerification);

        void UpdateSecret(string service, string key, string secret);

        void Register(string service, string key, string secret);
        
        bool HasUserByKey(string service, string key);

        bool HasUserByKey(string service, string key, out Guid? userID);
    }
}
