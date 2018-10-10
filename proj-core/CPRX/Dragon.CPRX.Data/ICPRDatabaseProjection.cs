using System.Data.Common;
using Dragon.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Dragon.CPRX
{
    public interface ICPRDatabaseProjection<T> : ICPRProjectionBase<T>
        where T: CPRCommand
    {
        void Project(IDbConnectionContextFactory connectionCtxFactory, ICPRContext ctx, T cmd, ILoggerFactory loggerFactory, IConfiguration config);
    }
}