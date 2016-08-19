using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dragon.Data.Interfaces;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    public class AppService : IAppService
    {
        private readonly IRepository<AppInfo> _consumerInfoRepository;
        private readonly IRepository<IdentityUserApp> _consumerUserRepository;

        public AppService(IRepository<AppInfo> consumerInfoRepository, IRepository<IdentityUserApp> consumerUserRepository)
        {
            _consumerInfoRepository = consumerInfoRepository;
            _consumerUserRepository = consumerUserRepository;
        }

        public IList<AppInfo> GetOtherRegisteredAppsInSameGroup(Guid userId, Guid appId)
        {
            return GetRegisteredAppsInSameGroup(userId, appId, true);
        }

        public IList<AppInfo> GetRegisteredAppsInSameGroup(Guid userId, Guid appId)
        {
            return GetRegisteredAppsInSameGroup(userId, appId, false);
        }

        private IList<AppInfo> GetRegisteredAppsInSameGroup(Guid userId, Guid appId, bool othersOnly)
        {
            var appInfo = _consumerInfoRepository.Get(appId.ToString());
            var appsInUse = _consumerUserRepository.GetByWhere(new Dictionary<string, object> {{"UserId", userId}}).Select(x => x.AppId).ToList();
            IList<AppInfo> consumerInfos = null;
            using (var c = ConnectionHelper.Open())
            {
                var query = "SELECT * FROM ConsumerInfo where AppId IN @Ids AND GroupId = @GroupId" + (othersOnly ? " AND AppId != @AppId" : "");
                consumerInfos = c.Query<AppInfo>(query, new {Ids = appsInUse, GroupId = appInfo.GroupId, AppId = appId}).ToList();
            }
            return consumerInfos;
        }
    }
}