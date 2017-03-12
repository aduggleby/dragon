using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public abstract class CPRCommand
    {
        public Guid ID { get; set; }
    }

    public abstract class CPRCommand<T>: CPRCommand
        where T : CPRCommand
    {
        public virtual IEnumerable<ICPRInterceptor<T>> Interceptors
        {
            get
            {
                yield break;
            }
        }

        public abstract IEnumerable<ICPRSecurityValidator<T>> SecurityValidators { get; }

        public virtual IEnumerable<ICPRValidator<T>> Validators
        {
            get
            {
                yield break;
            }
        }

        public abstract IEnumerable<ICPRProjection<T>> Projections { get; }
    }
}
