namespace Dragon.Diagnostics.Modules
{
    public class OperatingSystemModule : DiagnosticsModuleBase<OperatingSystemOptions>
    {
        protected override void ExecuteImpl(OperatingSystemOptions options)
        {
            DebugMessage(System.Environment.OSVersion.ToString());
        }
    }
}
