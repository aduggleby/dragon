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
    public class DemoInsertCommand : CommandBase
    {
        public DemoInsertCommand()
        {
            ID = Guid.NewGuid();
        }

        [Key]
        public Guid ID { get; set; }

        public string DemoString { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public Guid ContextID { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }

        public override bool ExecutionAllowed(IContext context)
        {
            return true; // Universal allow
        }
    }
}