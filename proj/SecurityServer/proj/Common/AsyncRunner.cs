using System;
using System.Threading.Tasks;
using AsyncBridge;

namespace Dragon.SecurityServer.Common
{
    public class AsyncRunner
    {
        public static T Run<T>(Task<T> task)
        {
            var result = default(T);
            using (var asyncHelper = AsyncHelper.Wait)
            {
                asyncHelper.Run(task, res => result = res );
            }
            return result;
        }

        public static T RunNoSynchronizationContext<T>(Func<Task<T>> action)
        {
            T result;
            using (new NoSynchronizationContextScope())
            {
                result = action().Result;
            }
            return result;
        }
   }
}
