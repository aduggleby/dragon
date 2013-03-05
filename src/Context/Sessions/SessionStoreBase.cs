using System;
using System.Collections.Concurrent;
using Dragon.Common.Extensions;
using Dragon.Common.Util;
using Dragon.Context.ReverseIPLookup;
using Dragon.Interfaces;
using StructureMap;

namespace Dragon.Context.Sessions
{
    public abstract class SessionStoreBase : ISessionStore
    {
        protected const string CONFIG_PREFIX = "Dragon.Context.Session.";
        protected const string CONFIG_DOREVERSEIPLOOKUP = CONFIG_PREFIX + "ReverseIPLookup";
        protected const string CONFIG_SLIDINGWINDOWSMINUTES = CONFIG_PREFIX + "SlidingWindowMinutes";

        protected readonly IReverseIPLookupService m_reverseLookupService;
        protected readonly CookieSession m_session;
        
        private static IConfiguration m_configuration;

        static SessionStoreBase()
        {
            m_configuration = ObjectFactory.GetInstance<IConfiguration>();
        }

        public SessionStoreBase(IReverseIPLookupService reverseLookupService)
        {
            m_session = new CookieSession();
            m_reverseLookupService = reverseLookupService;   
        }

        protected abstract SessionRecord GetSessionRecord();

        protected abstract void SaveSessionRecord(SessionRecord sessionRecord);

        protected void SetSessionData(SessionRecord sessionRecord)
        {
            sessionRecord.Hash = m_session.GetHashCode();

            // each request "touches" expires
            sessionRecord.Expires = DateTime.UtcNow.AddMinutes(SlidingWindowMinutes);

            SetLocationIfRequired(sessionRecord);
        }

        protected void SetLocationIfRequired(SessionRecord sessionRecord)
        {
            // perform ip -> location lookup if configured
            if (!sessionRecord.Hash.Equals(m_session.GetHashCode()))
            {
                if (m_configuration.IsTrue(CONFIG_DOREVERSEIPLOOKUP) && m_reverseLookupService != null)
                {
                    // if forwarded for avaiable try that first, otherwise just ip
                    sessionRecord.Location = !string.IsNullOrWhiteSpace(m_session.ForwardedForAddress)
                                                 ? (m_reverseLookupService.GetLocationString(m_session.ForwardedForAddress) ??
                                                    m_reverseLookupService.GetLocationString(m_session.IPAddress))
                                                 : (m_reverseLookupService.GetLocationString(m_session.IPAddress));
                }
            }
        }
        
        ///////////////////////////////////////////////////////////////////////
        
        public int SlidingWindowMinutes
        {
            get { return m_configuration.GetInt(CONFIG_SLIDINGWINDOWSMINUTES, 10); }
        }

        public ISession Session
        {
            get { return m_session; }
        }

        public Guid ConnectedUserID
        {
            get { return GetSessionRecord().UserID; }
            set
            {
                var s = GetSessionRecord();
                s.UserID = value;
                SaveSessionRecord(s);
            }
        }

        ///////////////////////////////////////////////////////////////////////

       
    }

    public class SessionRecord
    {
        public Guid SessionID;
        public DateTime Expires;
        public int Hash;
        public string Location;
        public Guid UserID;
    }
}
