using System.Collections.Generic;

namespace Dragon.CPRX
{
    public interface ICPRValidator
    {
        IEnumerable<CPRError> Validate(ICPRContext ctx, CPRCommand cmd);
    }
}