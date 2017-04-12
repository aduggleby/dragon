using System.Data.Common;
using Dragon.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Dragon.CPRX
{
    public interface ICPRDatabaseValidator<T>
        where T: CPRCommand
    {
        IEnumerable<CPRError> Validate(IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, T cmd, ILoggerFactory loggerFactory, IConfiguration config);
    }
}