using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Errors
{
    public class GeneralError : ErrorBase
    {
        public GeneralError()
        {
            
        }

        public GeneralError(string message)
        {
            this.PropertyName = string.Empty;
            this.Message = message;
        }
    }
}
