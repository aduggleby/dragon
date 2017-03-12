using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.CPRX
{
    public abstract class CommandBase
    {
        public virtual ICommandPersister Persitor { get; }

        public abstract ICommandSecurityPolicy<CommandBase> SecurityPolicy { get; }

        public abstract IEnumerable<ICommandValidator> Validators { get; }

        public abstract IEnumerable<ICommandProjection> Projections { get; }
    }
}
