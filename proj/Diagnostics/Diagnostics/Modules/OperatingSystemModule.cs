using System;

namespace Dragon.Diagnostics.Modules
{
    public class OperatingSystemModule : DiagnosticsModuleBase<OperatingSystemOptions>
    {
        protected override void ExecuteImpl(OperatingSystemOptions options)
        {
            DebugMessage(string.Format("{0} {1}bit", Environment.OSVersion, Environment.Is64BitOperatingSystem ? "64":"32"));
        }
    }
}
