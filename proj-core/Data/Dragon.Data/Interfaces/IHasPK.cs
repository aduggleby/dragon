using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Data.Interfaces
{
    public interface IHasPK<TKey>
    {
        TKey PK { get; set; }
    }
}
