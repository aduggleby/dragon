using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Context.Interfaces;
using Dragon.Data.Attributes;

namespace Dragon.Context.Sessions
{
    public class DragonSession : IDragonTable
    {
        [Key]
        public Guid SessionID { get; set; }

        [Length(200)]
        public int Hash { get; set; }

        [Length(200)]
        public string Location { get; set; }

        public Guid UserID { get; set; }

        public DateTime Expires { get; set; }

        public string Provider { get; set; }

        
    }
}
