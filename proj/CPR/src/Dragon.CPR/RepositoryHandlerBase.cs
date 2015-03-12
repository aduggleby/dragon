using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.CPR.Errors;
using Dragon.Data.Interfaces;

namespace Dragon.CPR
{
    public abstract class RepositoryHandlerBase<T> : HandlerBase<T>
        where T : CommandBase
    {
        public IRepository<T> Repository { get; set; }
    }
}
