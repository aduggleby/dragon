using System;
using Dragon.Security.Hmac.Module.Models;

namespace Dragon.Security.Hmac.Module.Repositories
{
    public interface IAppRepository
    {
        AppModel Get(Guid appId, Guid serviceId);
    }
}