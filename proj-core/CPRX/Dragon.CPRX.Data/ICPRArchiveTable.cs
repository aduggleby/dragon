using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public interface ICPRArchiveTable : ICPRTable
    {
        Guid LUID { get; set; }
        Guid CreatedByUserID { get; set; }
        Guid ModifiedByUserID { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime ModifiedOn { get; set; }

        Guid DeletedByUserID { get; set; }
        DateTime DeletedOn { get; set; }

    }
}
