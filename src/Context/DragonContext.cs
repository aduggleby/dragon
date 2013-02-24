using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;
using StructureMap;

namespace Dragon.Context
{
    public class DragonContext
    {
        private ISessionStore m_sessionStore;
        private IUserStore m_userStore;

        public DragonContext(
            ISessionStore sessionStore,
            IUserStore userStore)
        {
            m_sessionStore = sessionStore;
            m_userStore = userStore;
        }

        public static DragonContext Current
        {
            get
            {
                return ObjectFactory.GetInstance<DragonContext>();
            }
        }

        public static IPermissionStore PermissionStore
        {
            get
            {
                return ObjectFactory.GetInstance<IPermissionStore>();
            }
        }

        public static IProfileStore ProfileStore
        {
            get
            {
                return ObjectFactory.GetInstance<IProfileStore>();
            }
        }
        
        internal IUserStore UserStore
        {
            get
            {
                return m_userStore;
            }
        }

        public Guid CurrentUserID
        {
            get { return m_sessionStore.ConnectedUserID; }
        }

        public ISession Session
        {
            get { return m_sessionStore.Session; }
        }

        public IUser User
        {
            get { return m_userStore.User; }
        }

        public void Logout()
        {
            m_sessionStore.ConnectedUserID = Guid.Empty;
        }
    }
}
