using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public interface ICPRStore
    {
        void Persist(CPRCommand cmd, Guid executingUserID, DateTime utcBeganProjections);
    }
}
