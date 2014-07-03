using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Common;
using Dragon.Core.Configuration;
using Dragon.Interfaces;

namespace Dragon.Context.Users
{
    public class SqlUserStore : UserStoreBase
    {
        private string m_connStr;

        public SqlUserStore(ISessionStore sessionStore)
            : base(sessionStore)
        {
            Init();
        }

        protected override IEnumerable<IRegistration> LoadUser(Guid userID)
        {
            using (var conn = new SqlConnection(ConnectionStringManager.Value))
            {
                conn.Open();
                var param = new { UserID = userID };
                return conn.QueryFor<DragonRegistration>(SQL.SqlUserStore_GetByUserID, param);
            }
        }

        protected override IRegistration LoadRegistration(string service, string key)
        {
            using (var conn = new SqlConnection(ConnectionStringManager.Value))
            {
                conn.Open();
                var param = new { Service = service, Key = key };
                return conn.QueryFor<DragonRegistration>(SQL.SqlUserStore_GetByServiceAndKey, param).FirstOrDefault();
            }
        }

        protected override void Save(Guid userID, string service, string key, string hashedSaltedSecret, string newkey = null)
        {
            using (var conn = new SqlConnection(ConnectionStringManager.Value))
            {
                var existing = LoadRegistration(service, key);
                conn.Open();
                var sqlUser = new DragonRegistration()
                {
                    RegistrationID = Guid.NewGuid(),
                    UserID = userID,
                    Service = service,
                    Key = newkey??key, /* can rename key */
                    Secret = hashedSaltedSecret
                };

                if (existing != null)
                {
                    sqlUser.RegistrationID = existing.RegistrationID;

                    if (!existing.Key.Equals(key))
                        throw new InvalidOperationException(
                            "Trying to attach another account from the already connected service");
                    conn.ExecuteFor<DragonRegistration>(SQL.SqlUserStore_Update, sqlUser);

                }
                else
                {
                    conn.ExecuteFor<DragonRegistration>(SQL.SqlUserStore_Insert, sqlUser);
                }
            }
        }

        
    }
}
