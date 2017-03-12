using System.Collections.Generic;

namespace Dragon.CPRX
{
    public interface ICPRSecurityValidator
    {
        IEnumerable<CPRSecurityError> Authenticate(ICPRContext ctx, CPRCommand cmd);
    }
}