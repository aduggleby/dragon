using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Common;
using Dragon.Context.Permissions;
using Dragon.Context.ReverseIPLookup;
using Dragon.Interfaces;

namespace Dragon.Context.Sessions
{
    public class SqlSessionStore : InMemorySessionStore
    {

        public SqlSessionStore(ISession session, IReverseIPLookupService reverseLookupService)
            : base(session, reverseLookupService)
        {
      
        }

        protected override SessionRecord GetSessionRecord()
        {
            SessionRecord sessionRecord = null;
            if (true /* disabling in memory for a test todo */ || !base.TryGetSessionRecord(m_session.ID, out sessionRecord))
            {
                sessionRecord = GetSessionRecord(m_session.ID);
                if (sessionRecord == null)
                {
                    sessionRecord = new SessionRecord()
                        {
                            SessionID = m_session.ID
                        };
                    
                    SaveSessionRecord(sessionRecord);
                }
            }
            return sessionRecord;
        }

        protected virtual SessionRecord GetSessionRecord(Guid sessionID)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                return conn.Query<SessionRecord>(SQL.SqlSessionStore_Get, new {SessionID = sessionID}).FirstOrDefault();
            }
        }

        protected override void SaveSessionRecord(SessionRecord sessionRecord)
        {
            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();

                SetSessionData(sessionRecord);
                var p = new
                    {
                        Hash = sessionRecord.Hash,
                        SessionID = sessionRecord.SessionID,
                        Expires = sessionRecord.Expires,
                        Location = sessionRecord.Location,
                        UserID = sessionRecord.UserID
                    };
                if (conn.Execute(SQL.SqlSessionStore_Update, p)==0)
                {
                    conn.Execute(SQL.SqlSessionStore_Insert, p);
                }
            }

            base.SaveSessionRecord(sessionRecord);
        }

        protected override void RemoveSessionRecord(Guid sessionID)
        {
            base.RemoveSessionRecord(sessionID);

            using (var conn = new SqlConnection(StandardSqlStore.ConnectionString))
            {
                conn.Open();
                conn.Execute(SQL.SqlSessionStore_Delete, new { SessionID = sessionID });
            }
        }
    }
}
