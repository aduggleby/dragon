using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Web.Interfaces
{
    public interface ICommandSave<in T>
    {
        void Save(T obj);
    }
}
