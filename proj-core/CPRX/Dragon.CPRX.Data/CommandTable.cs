using Dragon.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX.Data
{
    [Table("Commands")]
    public class CommandTable
    {
        [Key]
        public Guid CommandID { get; set; }

        [Length("200")]
        public string Type { get; set; }

        [Length("MAX")]
        public string JSON { get; set; }

        public Guid UserID { get; set; }

        public DateTime Executed { get; set; }
    }
}
