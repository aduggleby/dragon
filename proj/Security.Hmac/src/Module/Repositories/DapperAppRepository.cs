using System;
using System.Collections.Generic;
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

        public AppModel Get(int id)
        {
            return _connection.Query<AppModel>(
                String.Format("SELECT * FROM {0} WHERE Id = @Id ORDER BY CreatedAt DESC", _tableName),
                new { Id = id }).FirstOrDefault();
        }

        public IEnumerable<AppModel> GetAll()
        {
            return _connection.Query<AppModel>(String.Format("SELECT * FROM {0}", _tableName));
        }

        public void Delete(int id)
        {
            _connection.Query(
                String.Format("DELETE FROM {0} WHERE Id = @Id", _tableName),
                new {Id = id});
        }

        public void Update(int id, AppModel user)
        {
            if (Get(id) == null)
            {
                throw new ArgumentException("App not found: " + id);
            }
            _connection.Query(
                String.Format(@"
                    UPDATE {0} SET
                    Name = @Name, 
                    AppId = @AppId, 
                    ServiceId = @ServiceId, 
                    Enabled = @Enabled, 
                    Secret = @Secret, 
                    CreatedAt = @CreatedAt 
                    WHERE id = @id", _tableName),
                new
                {
                    id,
                    user.Name,
                    user.ServiceId,
                    user.AppId,
                    user.Enabled,
                    user.CreatedAt,
                    user.Secret
                });
        }

        public int Insert(AppModel app)
        {
            return _connection.Query<int>(
                String.Format(@"
                    DECLARE @InsertedRows AS TABLE (Id int);
                    INSERT INTO {0} OUTPUT Inserted.Id INTO @InsertedRows VALUES (@AppId, @ServiceId, @Secret, @Enabled, @CreatedAt, @Name);
                    SELECT Id FROM @InsertedRows;", _tableName),
                new {app.AppId, app.ServiceId, app.Secret, app.Enabled, app.CreatedAt, app.Name}).Single();
        }
    }
}
