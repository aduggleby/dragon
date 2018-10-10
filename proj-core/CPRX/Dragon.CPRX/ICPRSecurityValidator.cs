using System.Collections.Generic;

namespace Dragon.CPRX
{
    public interface ICPRSecurityValidator<in T>
        where T : CPRCommand
    {
        IEnumerable<CPRSecurityError> Authenticate(ICPRContext ctx, T cmd);
    }
}