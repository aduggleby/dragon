using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Web.Interfaces;

namespace Dragon.Web.Defaults
{
    public class DefaultContext : IContext
    {
        public Guid UserID { get; private set; }
        public Guid ContextID { get; private set; }

        public void Log(string msg)
        {
            var m = string.Format("{0}: {1}", DateTime.UtcNow.ToString("HH:mm:ss:ffff"), msg);
            Debug.WriteLine(m);            
            Trace.WriteLine(m);
        }

        public DefaultContext()
        {
            UserID = Guid.Empty;
            ContextID = Guid.Empty;
        }
    }
}
