using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Context.Interfaces
{
    public interface IProfileStore
    {
        T GetProperty<T>(Guid userID, string key);
        void SetProperty(Guid userID, string key, object value);
    }
}
