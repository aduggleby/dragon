using System;
using Dragon.Common.Attributes.Data;
using Dragon.CPR.Attributes;

namespace Dragon.CPR
{
    public abstract class CommandBase : PersistableBase
    {
        [Hide]
        public Guid CommandID { get; set; }

        [Hide]
        public Guid ExecutingUserID { get; set; }

    }
}
