using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Common;
using Dragon.Context.ReverseIPLookup;
using Dragon.Core.Configuration;
using Dragon.Interfaces;
using Dragon.SQL.Repositories;

namespace Dragon.Context.Sessions
{
    public class SqlSessionStore : InMemorySessionStore
    {

        public SqlSessionStore(ISession session, IReverseIPLookupService reverseLookupService)
            : base(session, reverseLookupService)
        {

        }

        protected override DragonSession GetSessionRecord()
        {
            DragonSession sessionRecord = null;
            if (true /* disabling in memory for a test todo */ || !base.TryGetSessionRecord(m_session.ID, out sessionRecord))
            {
                sessionRecord = GetSessionRecord(m_session.ID);
                if (sessionRecord == null)
                {
                    sessionRecord = new DragonSession()
                        {
                            SessionID = m_session.ID
                        };

                    SaveSessionRecord(sessionRecord);
                }
            }
            return sessionRecord;
        }

        protected virtual DragonSession GetSessionRecord(Guid sessionID)
        {
            using (var conn = ConnectionHelper.Open())
            {
                return conn.QueryFor<DragonSession>(SQL.SqlSessionStore_Get, new { SessionID = sessionID }).FirstOrDefault();
            }
        }

        protected override void SaveSessionRecord(DragonSession sessionRecord)
        {
            using (var conn = ConnectionHelper.Open())
            {

                SetSessionData(sessionRecord);
                var p = new
                    {
                        Hash = sessionRecord.Hash,
                        SessionID = sessionRecord.SessionID,
                        Expires = sessionRecord.Expires,
                        Location = sessionRecord.Location,
                        UserID = sessionRecord.UserID
                    };
                if (conn.ExecuteFor<DragonSession>(SQL.SqlSessionStore_Update, p) == 0)
                {
                    conn.ExecuteFor<DragonSession>(SQL.SqlSessionStore_Insert, p);
                }
            }

            base.SaveSessionRecord(sessionRecord);
        }

        protected override void RemoveSessionRecord(Guid sessionID)
        {
            base.RemoveSessionRecord(sessionID);

            using (var conn = ConnectionHelper.Open())
            {
                conn.ExecuteFor<DragonSession>(SQL.SqlSessionStore_Delete, new { SessionID = sessionID });
            }
        }
    }
}
