using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Context.Sessions
{
    public class InMemorySession : ISession
    {
        private Guid m_id;
        private Guid m_userid;

        public InMemorySession(Guid id)
        {
            m_id = id;
            m_userid = Guid.NewGuid();
        }

        public Guid ID
        {
            get { return m_id; }
        }

        public Guid UserID
        {
            get { return m_userid; }
        }


        public string IPAddress
        {
            get { return "127.0.0.1"; }
        }

        public string ForwardedForAddress
        {
            get { return null; }
        }
    }
}
