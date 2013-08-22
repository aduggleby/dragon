using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Interfaces
{
    public interface ISetupRepository 
    {
        void EnsureTableExists<T>() where T : class;
    }
}
