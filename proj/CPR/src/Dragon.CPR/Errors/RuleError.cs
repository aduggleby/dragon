using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Errors
{
    public class RuleError : ErrorBase
    {
        public RuleError()
        {
            
        }

        public RuleError(string property, string message)
        {
            this.PropertyName = property;
            this.Message = message;
        }
    }
}
