using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragon.Data.Attributes;
using Dragon.Web.Interfaces;

namespace Dragon.Web.Demo.CPR
{
    public class DemoDeleteCommand : CommandBase
    {
        [Key]
        public Guid ID { get; set; }

        public override bool ExecutionAllowed(IContext context)
        {
            return false; // Requires admin
        }
    }
}