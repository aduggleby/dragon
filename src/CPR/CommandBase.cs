using System;
using Dragon.Common.Attributes.Data;
using Dragon.CPR.Attributes;
using Dragon.CPR.Attributes.UI;

namespace Dragon.CPR
{
    public abstract class CommandBase : PersistableBase
    {
        [Hide]
        public Guid CommandID { get; set; }

        [Hide]
        public Guid UserID { get; set; }

    }
}
