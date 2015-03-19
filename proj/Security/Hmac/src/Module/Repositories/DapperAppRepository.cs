using System;
using System.Data;
using System.Linq;
using Dapper;
using Dragon.Security.Hmac.Module.Models;

namespace Dragon.Security.Hmac.Module.Repositories
{
    public class DapperAppRepository : IAppRepository
    {
        private readonly IDbConnection _connection;
        private readonly string _tableName;

        public DapperAppRepository(IDbConnection connection, string tableName)
        {
            _connection = connection;
            _tableName = tableName;
        }

        public AppModel Get(Guid appId, Guid serviceId)
        {
            return _connection.Query<AppModel>(
                String.Format("SELECT * FROM {0} WHERE AppId = @AppId AND ServiceId = @ServiceId ORDER BY CreatedAt DESC", _tableName), 
                new {AppId = appId, ServiceId = serviceId}).FirstOrDefault();
        }
    }
}   