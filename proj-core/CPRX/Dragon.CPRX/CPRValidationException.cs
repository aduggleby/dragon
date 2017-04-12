
using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public class CPRValidationException : Exception
    {
        public IEnumerable<CPRError> Errors { get; set; }

        public CPRValidationException(IEnumerable<CPRError> errors)
        {
            Errors = errors;
        }
    }
}
