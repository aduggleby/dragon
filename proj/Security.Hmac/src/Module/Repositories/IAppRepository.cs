using System;
using System.Collections.Generic;
using Dragon.Security.Hmac.Module.Models;

namespace Dragon.Security.Hmac.Module.Repositories
{
    public interface IAppRepository
    {
        AppModel Get(Guid appId, Guid serviceId);
        AppModel Get(int id);
        IEnumerable<AppModel> GetAll();
        int Insert(AppModel app);
        void Delete(int id);
        void Update(int id, AppModel user);
    }
}