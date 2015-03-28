using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Web.Interfaces
{
    public interface IContext
    {
        Guid UserID { get; }
        Guid ContextID { get; }

        void Log(string msg);
    }
}
