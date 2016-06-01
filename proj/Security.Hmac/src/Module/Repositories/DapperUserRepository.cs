using System;
using System.Collections.Generic;
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

        public UserModel Get(long id)
        {
            return _connection.Query<UserModel>(
                String.Format("SELECT * FROM {0} WHERE Id = @Id ORDER BY CreatedAt DESC", _tableName),
                new { Id = id }).FirstOrDefault();
        }

        public long Insert(UserModel user)
        {
            return _connection.Query<long>(String.Format(@"
                DECLARE @InsertedRows AS TABLE (Id int);
                INSERT INTO {0} OUTPUT Inserted.Id INTO @InsertedRows VALUES (@UserId, @AppId, @ServiceId, @Enabled, @CreatedAt);
                SELECT Id FROM @InsertedRows;", _tableName), new
            {
                user.UserId,
                user.ServiceId,
                user.AppId,
                user.Enabled,
                user.CreatedAt
            }).Single();
        }

        public IEnumerable<UserModel> GetAll()
        {
            return _connection.Query<UserModel>(
                String.Format("SELECT * FROM {0} ", _tableName));
        }

        public void Delete(long id)
        {
            _connection.Query(
                String.Format("DELETE FROM {0} WHERE Id = @Id", _tableName),
                new { Id = id });
        }

        public void Update(long id, UserModel user)
        {
            if (Get(id) == null)
            {
                throw new ArgumentException("User not found: " + id);
            }
            _connection.Query(
                String.Format(@"
                    UPDATE {0} SET
                    UserId = @UserId, 
                    AppId = @AppId, 
                    ServiceId = 
                    @ServiceId, 
                    Enabled = @Enabled, 
                    CreatedAt = @CreatedAt 
                    WHERE id = @id", _tableName),
                new
                {
                    id,
                    user.UserId,
                    user.ServiceId,
                    user.AppId,
                    user.Enabled,
                    user.CreatedAt
                });
        }
    }
}