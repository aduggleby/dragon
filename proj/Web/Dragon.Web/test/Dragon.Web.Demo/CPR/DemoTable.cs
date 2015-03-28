using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragon.Data.Attributes;

namespace Dragon.Web.Demo.CPR
{
    public class DemoTable : TableBase
    {
        [Key]
        public Guid ID { get; set; }

        public string DemoString { get; set; }
    }
}