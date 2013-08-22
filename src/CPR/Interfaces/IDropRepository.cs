using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Interfaces
{
    public interface IDropRepository
    {
        void DropTableIfExists<T>() where T : class;
        void DropTableIfExists(string name);
    }
}
