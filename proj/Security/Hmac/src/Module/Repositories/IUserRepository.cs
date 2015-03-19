using System;
using Dragon.Security.Hmac.Module.Models;

namespace Dragon.Security.Hmac.Module.Repositories
{
    public interface IUserRepository
    {
        UserModel Get(Guid userId, Guid serviceId);
        void Insert(UserModel user);
    }
}