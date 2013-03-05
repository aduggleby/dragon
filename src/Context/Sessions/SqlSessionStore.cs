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
        private string m_connStr;

        public SqlSessionStore(ISession session, IReverseIPLookupService reverseLookupService)
            : base(session, reverseLookupService)
        {
            var connStrEntry = ConfigurationManager.ConnectionStrings[Constants.DEFAULT_CONNECTIONSTRING_KEY];

            if (connStrEntry == null || string.IsNullOrWhiteSpace(connStrEntry.ConnectionString))
            {
                throw Ex.For(SQL.SqlStores_Exception_ConnectionStringNotSet,
                             Constants.DEFAULT_CONNECTIONSTRING_KEY);
            }

            m_connStr = connStrEntry.ConnectionString;
        }

        protected override SessionRecord GetSessionRecord()
        {
            SessionRecord sessionRecord;
            if (!base.TryGetSessionRecord(m_session.ID, out sessionRecord))
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
            using (var conn = new SqlConnection(m_connStr))
            {
                conn.Open();
                return conn.Query<SessionRecord>(SQL.SqlSessionStore_Get, new {SessionID = sessionID}).FirstOrDefault();
            }
        }

        protected override void SaveSessionRecord(SessionRecord sessionRecord)
        {
            using (var conn = new SqlConnection(m_connStr))
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
            using (var conn = new SqlConnection(m_connStr))
            {
                conn.Open();
                conn.Execute(SQL.SqlSessionStore_Delete, new { SessionID = sessionID });
            }
        }
    }
}
