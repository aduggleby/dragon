using System;
using System.Data;
using System.Linq;
using Dapper;
using Dragon.Security.Hmac.Module.Models;

namespace Dragon.Security.Hmac.Module.Repositories
{
    public class DapperUserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;
        private readonly string _tableName;

        public DapperUserRepository(IDbConnection connection, string tableName)
        {
            _connection = connection;
            _tableName = tableName;
        }

        public UserModel Get(Guid userId, Guid serviceId)
        {
            return _connection.Query<UserModel>(
                String.Format("SELECT * FROM {0} WHERE UserId = @UserId AND ServiceId = @ServiceId ORDER BY CreatedAt DESC", _tableName), 
                new {UserId = userId, ServiceId = serviceId}).FirstOrDefault();
        }

        public void Insert(UserModel user)
        {
            _connection.Execute(String.Format("INSERT INTO {0} VALUES (@UserId, @AppId, @ServiceId, @Enabled, @CreatedAt)", _tableName), new
            {
                user.UserId,
                user.ServiceId,
                user.AppId,
                user.Enabled,
                user.CreatedAt
            });
        }
    }
}