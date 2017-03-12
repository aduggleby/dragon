using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX
{
    public class CPRExecutionResult
    {
        private List<CPRError> m_errors;

        public CPRExecutionResult()
        {
            m_errors = new List<CPRError>();
        }

        public bool Success { get; set; }
        
        public IEnumerable<CPRError> Errors
        {
            get
            {
                return m_errors;
            }
        }

        public void AddError(CPRError err)
        {
            m_errors.Add(err);
        }

        public void AddErrors(IEnumerable<CPRError> errs)
        {
            m_errors.AddRange(errs);
        }
    }
}
