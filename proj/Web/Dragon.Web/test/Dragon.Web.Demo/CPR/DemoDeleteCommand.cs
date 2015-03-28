using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragon.Data.Attributes;

namespace Dragon.Web.Demo.CPR
{
    public class DemoDeleteCommand : CommandBase
    {
        [Key]
        public Guid ID { get; set; }

        public override bool ExecutionAllowed()
        {
            return false; // Requires admin
        }
    }
}