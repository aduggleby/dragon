using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dragon.Context.Exceptions;
using Dragon.Interfaces;
using Dragon.Common.Util;

namespace Dragon.Context.Users
{
    public abstract class UserStoreBase : IUserStore
    {
        private ISessionStore m_sessionStore;

        public UserStoreBase(ISessionStore sessionStore)
        {
            m_sessionStore = sessionStore;

        }

        protected void Init()
        {

        }

        protected abstract IEnumerable<IRegistration> LoadUser(Guid userID);

        protected abstract IRegistration LoadRegistration(string service, string key);

        protected abstract void Save(Guid userID, string service, string key, string secret);

        public virtual bool HasUserByKey(string service, string key, out Guid? userID)
        {
            userID = null;
            var user = LoadRegistration(service, key);
            if (user != null)
            {
                userID = user.UserID;
            }

            return user != null;
        }

        public virtual bool HasUserByKey(string service, string key)
        {
            Guid? g;
            return HasUserByKey(service, key, out g);
        }

        public bool TryLogin(string service, string key, Func<string, bool> secretVerification)
        {
            EnsureArgumentsMeetLengthConstraints(service, key, string.Empty);

            m_sessionStore.ConnectedUserID = Guid.Empty;

            var user = LoadRegistration(service, key);

            if (user == null)
            {
                return false;
            }

            if (secretVerification(user.Secret))
            {
                m_sessionStore.ConnectedUserID = user.UserID;
            }

            return !m_sessionStore.ConnectedUserID.Equals(Guid.Empty);
        }

        public void UpdateSecret(string service, string key, string secret)
        {
            EnsureArgumentsMeetLengthConstraints(service, key, string.Empty);

            var user = LoadRegistration(service, key);

            if (user != null && !user.Secret.Equals(secret))
            {
                if (!user.UserID.Equals(m_sessionStore.ConnectedUserID))
                {
                    throw new InvalidOperationException("Cannot update another users secret");
                }

                Save(user.UserID, service, key, secret);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="secret">The secret must be hashed by the caller if required. No hashing from this point on!</param>
        public void Register(string service, string key, string secret)
        {
            EnsureArgumentsMeetLengthConstraints(service, key, secret);

            var userID = m_sessionStore.ConnectedUserID;
            if (userID.Equals(Guid.Empty))
            {
                // new user
                userID = Guid.NewGuid();
                m_sessionStore.ConnectedUserID = userID;
            }

            var existingUser = LoadRegistration(service, key) ?? LoadUser(userID).FirstOrDefault();
            if (existingUser != null)
            {
                throw new ServiceAlreadyConnectedToUserException();
            }

            Save(userID, service, key, secret);
        }

        public void EnsureArgumentsMeetLengthConstraints(string service, string key, string secret)
        {
            if (service.Length > 100) throw new ArgumentException("Service must be less than 100 characters.");
            if (key.Length > 200) throw new ArgumentException("Service must be less than 200 characters.");
            if (secret.Length > 200) throw new ArgumentException("Secret must be less than 200 characters.");
        }

        public IUser User
        {
            get { throw new NotImplementedException(); }
        }
    }
}
