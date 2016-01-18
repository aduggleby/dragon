using System;
using System.Diagnostics;

namespace Dragon.Diagnostics
{
    public abstract class DiagnosticsModuleBase<TOptions> : IDiagnosticModule<TOptions> where TOptions : DiagnosticsOptionsBase
    {
        protected string Log;

        public string Execute(TOptions options)
        {
            if (options.Disabled)
            {
                DebugMessage(GetType().Name + " is disabled, skipping.");
                return Log;
            }
            ExecuteImpl(options);
            return Log;
        }

        protected abstract void ExecuteImpl(TOptions options);

        protected void DebugMessage(string message)
        {
            Debug.WriteLine(message);
            Log += message + Environment.NewLine;
        }

    }
}
