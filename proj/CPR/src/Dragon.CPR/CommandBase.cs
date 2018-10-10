using System;
using Dragon.CPR.Attributes;
using Dragon.Data.Attributes;

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
