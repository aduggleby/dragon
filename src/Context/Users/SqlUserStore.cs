using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Common;
using Dragon.Interfaces;

namespace Dragon.Context.Users
{
    public class SqlUserStore : UserStoreBase
    {
        private string m_connStr;

        public SqlUserStore(ISessionStore sessionStore):base(sessionStore)
        {
           Init();
        }

        protected override IEnumerable<IRegistration> LoadUser(Guid userID)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { UserID = userID };
                return conn.Query<SQLUser>(SQL.SqlUserStore_GetByUserID, param);
            }
        }

        protected override IRegistration LoadRegistration(string service, string key)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { Service = service, Key = key };
                return conn.Query<SQLUser>(SQL.SqlUserStore_GetByServiceAndKey, param).FirstOrDefault();
            }
        }

        protected override void Save(Guid userID, string service, string key, string hashedSaltedSecret)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                var existing = LoadRegistration(service, key);
                conn.Open();
                var sqlUser = new SQLUser()
                {
                    RegistrationID = Guid.NewGuid(),
                    UserID = userID,
                    Service = service,
                    Key = key,
                    Secret = hashedSaltedSecret
                };

                if (existing != null)
                {
                    sqlUser.RegistrationID = existing.RegistrationID;

                    if (!existing.Key.Equals(key))
                        throw new InvalidOperationException(
                            "Trying to attach another account from the already connected service");
                    conn.Execute(SQL.SqlUserStore_Update, sqlUser);

                }
                else
                {
                    conn.Execute(SQL.SqlUserStore_Insert, sqlUser);
                }
            }
        }

        private class SQLUser : IRegistration 
        {
            public Guid RegistrationID { get; set; }
            public Guid UserID { get; set; }
            public string Service { get; set; }
            public string Key { get; set; }
            public string Secret { get; set; }
        }
    }
}
