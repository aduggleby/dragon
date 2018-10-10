using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Dragon.Context.Interfaces;
using StructureMap;

namespace Dragon.Context
{
    public class DragonContext
    {
        private ISessionStore m_sessionStore;
        private IUserStore m_userStore;

        public const string PROFILEKEY_SERVICE = "dragon.context.service";

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

        private Guid? m_currentUserID;

        public ISession m_session;

        public IUser m_user;

        public Guid CurrentUserID
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var uid = HttpContext.Current.Items["DragonContext_CurrentUserID"] as Guid?;
                    if (uid == null)
                    {
                        uid = m_sessionStore.ConnectedUserID;
                        HttpContext.Current.Items["DragonContext_CurrentUserID"] = uid;
                    }
                    return uid.Value;
                }
                else
                {
                    return m_sessionStore.ConnectedUserID;
                }
            }
        }

        public ISession Session
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var session = HttpContext.Current.Items["DragonContext_Session"] as ISession;
                    if (session == null)
                    {
                        session = m_sessionStore.Session;
                        HttpContext.Current.Items["DragonContext_Session"] = session;
                    }
                    return session;
                }
                else
                {
                    return m_sessionStore.Session;
                }
            }
        }

        public IUser User
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var user = HttpContext.Current.Items["DragonContext_User"] as IUser;
                    if (user == null)
                    {
                        user = m_userStore.User;
                        HttpContext.Current.Items["DragonContext_User"] = user;
                    }
                    return user;
                }
                else
                {
                    return m_userStore.User;
                }
            }
        }

        public void Logout()
        {
            HttpContext.Current.Items.Remove("DragonContext_User");
            HttpContext.Current.Items.Remove("DragonContext_Session");
            HttpContext.Current.Items.Remove("DragonContext_CurrentUserID");

            m_sessionStore.ConnectedUserID = Guid.Empty;
        }
    }
}
