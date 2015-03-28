using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Web.Interfaces
{
    public interface ICommandAfterSave<in T>
    {
        void AfterSave(T obj);
    }
}
