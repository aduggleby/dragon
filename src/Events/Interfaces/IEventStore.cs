using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Events.Interfaces
{
    public interface IEventStore
    {
        void Save(IEvent @event);
        void Get(, int? latest = null);
    }
}
