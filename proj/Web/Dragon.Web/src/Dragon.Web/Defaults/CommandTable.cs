using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Data.Attributes;

namespace Dragon.Web.Defaults
{
    [Table("_Command")]
    public class CommandTable
    {
        [Length(200)]
        public string Type { get; set; }

        [Length("MAX")]
        public string Object { get; set; }
        
        public Guid UserID { get; set; }
    }
}
