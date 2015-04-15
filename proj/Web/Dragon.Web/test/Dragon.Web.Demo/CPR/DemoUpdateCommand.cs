using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragon.Data.Attributes;
using Dragon.Web.Attributes;
using Dragon.Web.Interfaces;

namespace Dragon.Web.Demo.CPR
{
    [PersistedAs(typeof(DemoTable))]

    public class DemoUpdateCommand : CommandBase
    {
        [Key]
        public Guid ID { get; set; }

        public string DemoString { get; set; }

        public override bool ExecutionAllowed(IContext context)
        {
            return false; // Requires user
        }
    }
}