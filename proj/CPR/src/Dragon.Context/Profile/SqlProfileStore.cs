using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Context.Users;
using Dragon.Data.Repositories;

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
            using (var conn = ConnectionHelper.Open())
            {
                var param = new { UserID = userID, Key = key };
                var candidate = conn.QueryFor<DragonProfile>(SQL.SqlProfileStore_Get, param).FirstOrDefault();
                return candidate == null ? null : candidate.Value;
            }
        }

        protected override void SetPropertyInternal(Guid userID, string key, string val)
        {
            using (var conn = ConnectionHelper.Open())
            {
                var param = new { LID = Guid.NewGuid(), UserID = userID, Key = key, Value = val };
                if (conn.ExecuteFor<DragonProfile>(SQL.SqlProfileStore_Update, param) == 0)
                {
                    conn.ExecuteFor<DragonProfile>(SQL.SqlProfileStore_Insert, param);
                }
            } 
        }
    }
}
