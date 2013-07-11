using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Dragon.Core.Sql;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class SqlNotificationStore : INotificationStore
    {
        public void AddNotification(INotification notification)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                conn.Execute(SQL.SQLNotificationStore_Insert, notification);
            }
        }

        public IEnumerable<INotification> GetAllNotifications()
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new {};
                return conn.Query<Notification>(SQL.SQLNotificationStore_GetAll, param);
            }
        }

    }
}
