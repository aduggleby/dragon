using System.Collections.Generic;

namespace Dragon.CPRX
{
    public interface ICPRValidator<in T> where T: CPRCommand
    {
        IEnumerable<CPRError> Validate(ICPRContext ctx, CPRCommand cmd);
    }
}