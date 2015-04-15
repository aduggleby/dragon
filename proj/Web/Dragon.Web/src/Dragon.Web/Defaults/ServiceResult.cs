using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Web.Defaults
{
    public class ServiceResult<T>
    {
        public T Payload { get; private set; }

        public ServiceResult(T payload)
        {
            Payload = payload;
        }
    }

    public static class ServiceResultExtensions
    {
        public static bool WasSuccessfull(this ServiceResult<bool> sr)
        {
            return sr.Payload;
        }
    }
}
