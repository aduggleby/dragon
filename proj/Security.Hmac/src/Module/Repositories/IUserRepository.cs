using System;
using System.Collections.Generic;
using Dragon.Security.Hmac.Module.Models;

namespace Dragon.Security.Hmac.Module.Repositories
{
    public interface IUserRepository
    {
        UserModel Get(Guid userId, Guid serviceId);
        UserModel Get(long id);
        IEnumerable<UserModel> GetAll();
        long Insert(UserModel user);
        void Delete(long id);
        void Update(long id, UserModel user);
    }
}