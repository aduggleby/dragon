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

        public InMemorySession(Guid id)
        {
            m_id = id;
        }

        public Guid ID
        {
            get { return m_id; }
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
