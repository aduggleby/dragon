using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public interface ICPRContextFetcher
    {
        ICPRContext GetCurrentContext();
    }
}
