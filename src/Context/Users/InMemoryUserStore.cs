using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Context.Users
{
    public class InMemoryUserStore : IUserStore
    {
        private User m_user;

        public InMemoryUserStore()
        {
            m_user = new User();
            m_user.Key = "userkey";
            
        }

        public IUser User
        {
            get { return m_user; }
        }

        public bool Impersonate(Guid userID)
        {
            return true;
        }

        public bool TryLogin(string service, string key, Func<string, bool> secretVerification)
        {
            return true;
        }
        
        public void UpdateSecret(string service, string key, string secret)
        {
            // NOP
        }

        public void UpdateKey(string service, string oldkey, string newkey)
        {
            // NOP
            return;
        }

        public bool UpdateSecret(string service, string key, Func<string, bool> secretVerification, string secret)
        {
            // NOP
            return true;
        }
        public bool UpdateSecret(Guid userID, Func<string, bool> secretVerification, string secret)
        {
            // NOP
            return true;
        }
        public void Register(string service, string key, string secret)
        {
            // NOP
        }

        public bool HasUserByKey(string service, string key)
        {
            return false;
        }

        public bool HasUserByKey(string service, string key, out Guid? userID)
        {
            userID = Guid.NewGuid();
            return false;
        }
    }
}
