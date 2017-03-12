using System;
using System.Threading;

namespace Dragon.SecurityServer.Common
{
    /// <summary>
    /// See <a href="http://stackoverflow.com/questions/28305968/use-task-run-in-synchronous-method-to-avoid-deadlock-waiting-on-async-method">this link</a> for more information.
    /// </summary>
    public class NoSynchronizationContextScope : IDisposable
    {
        private readonly SynchronizationContext _synchronizationContext;
        public NoSynchronizationContextScope()
        {
            _synchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
        }
        public void Dispose()
        {
            SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
        }
    }
}
