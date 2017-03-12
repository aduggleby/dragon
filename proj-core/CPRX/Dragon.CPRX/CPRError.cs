using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public class CPRError
    {
        public string Message { get; set; }
    }

    public class CPRSecurityError : CPRError
    {

    }

    public class CPRValidationError : CPRError
    {

    }
    
}
