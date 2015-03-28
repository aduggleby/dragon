using System;
using System.Collections.Generic;
using System.Linq;
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
}
