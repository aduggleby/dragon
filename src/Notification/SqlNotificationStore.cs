using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using Dapper;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class SqlNotificationStore : INotificationStore
    {
        private readonly string _connectionString;

        public SqlNotificationStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Guid userId, INotification notification)
        {
            SqlExecute(SQL.SQLNotificationStore_Add, new
            {
                UserID = userId,
                NotificationID = notification.ID,
                notification.TypeKey,
                notification.Subject,
                notification.LanguageCode,
                notification.Dispatched,
                Parameter = new JavaScriptSerializer().Serialize(notification.Parameter)
            });
        }

        public IEnumerable<INotification> GetAll(Guid userID)
        {
            return SqlQuery(SQL.SQLNotificationStore_GetAll, new[] { new SqlParameter("@UserID", userID) });
        }

        public IEnumerable<INotification> GetAllUndispatched(Guid userID)
        {
            return SqlQuery(SQL.SQLNotificationStore_GetAllDispatched, new[]
            {
                new SqlParameter("@UserID", userID),
                new SqlParameter("@Dispatched", false)
            });
        }

        public void SetDispatched(Guid notificationID)
        {
            SqlExecute(SQL.SQLNotificationStore_UpdateDispatched, new {Dispatched = true, NotificationID = notificationID});
        }

        public void SetAllDispatched(Guid userID)
        {
            SqlExecute(SQL.SQLNotificationStore_UpdateAllDispatched, new {UserID = userID, Dispatched = true});
        }

        # region helpers

        private void SqlExecute(string sql, object param)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute(sql, param);
            }
        }

        private IEnumerable<INotification> SqlQuery(string sql, SqlParameter[] parameter)
        {
            IList<INotification> notifications = new List<INotification>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameter);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notifications.Add(CreateNotification(reader));
                        }
                    }
                }
            }
            return notifications;
        }

        private string GetString(SqlDataReader reader, string column)
        {
            return reader.GetString(reader.GetOrdinal(column));
        }

        private Notification CreateNotification(SqlDataReader reader)
        {
            return new Notification
            {
                ID = reader.GetGuid(reader.GetOrdinal("NotificationID")),
                LanguageCode = GetString(reader, "LanguageCode"),
                Subject = GetString(reader, "Subject"),
                TypeKey = GetString(reader, "TypeKey"),
                Dispatched = reader.GetBoolean(reader.GetOrdinal("Dispatched")),
                Parameter = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(
                    GetString(reader, "Parameter")
                )
            };
        }

        # endregion
    }
}
