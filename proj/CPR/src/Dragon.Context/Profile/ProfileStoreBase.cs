using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Context.Interfaces;

namespace Dragon.Context.Profile
{
    public abstract class ProfileStoreBase : StoreBase, IProfileStore
    {
        public ProfileStoreBase()
        {
          
        }

        protected abstract string GetPropertyInternal(Guid userID, string key);

        protected abstract void SetPropertyInternal(Guid userID, string key, string val);

        public T GetProperty<T>(Guid userID, string key)
        {
            var value = GetPropertyInternal(userID, key);

            if (string.IsNullOrWhiteSpace(value))
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void SetProperty(Guid userID, string key, object val)
        {
            SetPropertyInternal(userID, key, val.ToString());
        }

        
    }
}
