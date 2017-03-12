using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public abstract class CPRCommand
    {
        public virtual IEnumerable<CPRInterceptor> Intercept(ICPRContext ctx)
        {
            yield return new CPRTableInterceptor();
        }

        public abstract IEnumerable<ICPRSecurityValidator> SecurityValidators { get; }

        public virtual IEnumerable<ICPRValidator> Validators
        {
            get
            {
                yield break;
            }
        }

        public abstract IEnumerable<ICPRProjection> Projections { get; }
    }
}
