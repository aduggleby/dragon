using System;
using System.Collections.Generic;
using Dragon.SecurityServer.AccountSTS.Models;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    public interface IAppService
    {
        IList<AppInfo> GetOtherRegisteredAppsInSameGroup(Guid userId, Guid appId);
        IList<AppInfo> GetRegisteredAppsInSameGroup(Guid userId, Guid appId);
        bool IsRegisteredForApp(Guid userId, Guid appId);
    }
}
