using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Common;
using Dragon.Context.Users;
using Dragon.Interfaces;

namespace Dragon.Context.Profile
{
    public class SqlProfileStore : ProfileStoreBase
    {
        public SqlProfileStore()
            : base()
        {
        
        }

        protected override string GetPropertyInternal(Guid userID, string key)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { UserID = userID, Key = key };
                var candidate = conn.Query(SQL.SqlProfileStore_Get, param).FirstOrDefault();
                return candidate == null ? null : candidate.Value;
            }
        }

        protected override void SetPropertyInternal(Guid userID, string key, string val)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                var param = new { UserID = userID, Key = key, Value = val };
                if (conn.Execute(SQL.SqlProfileStore_Update, param) == 0)
                {
                    conn.Execute(SQL.SqlProfileStore_Insert, param);
                }
            } 
        }
    }
}
