using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Web
{
    public abstract class TableBase
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public Guid ContextID { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
