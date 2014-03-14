using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Common.Attributes.Data;
using Dragon.Interfaces.Tasks;

namespace Dragon.Tasks
{
    [Table("DragonTask")]
    public class Task : ITask
    {
        public Guid TaskID { get; set; }
        public Guid UserID { get; set; }
        public string Description { get; set; }
        public byte[] Payload { get; set; }
    }
}
