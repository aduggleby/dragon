using System;

namespace Dragon.CPRX
{
    public interface ICPRInterceptor<in T>
        where T : CPRCommand
    {
        void Intercept(ICPRContext ctx, T cmd);
    }
}