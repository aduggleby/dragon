using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces
{
    public interface IProfileStore
    {
        T GetProperty<T>(Guid userID, string key);
    }
}
