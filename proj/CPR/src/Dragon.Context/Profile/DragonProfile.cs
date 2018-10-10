using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Context.Interfaces;
using Dragon.Data.Attributes;

namespace Dragon.Context.Profile
{
    public class DragonProfile:IDragonTable
    {
        public DragonProfile()
        {
            LID = Guid.NewGuid();
        }

        [Key]
        public Guid LID { get; set; }

        public Guid UserID { get; set; }
        public string Key { get; set; }

        [Length(400)]
        public string Value { get; set; }

    }
}
