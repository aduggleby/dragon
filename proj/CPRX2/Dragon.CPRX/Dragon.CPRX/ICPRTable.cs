using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public interface ICPRTable
    {
        Guid CreatedByUserID { get; set; }
        Guid ModifiedByUserID { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime ModifiedOn { get; set; }
    }
}
