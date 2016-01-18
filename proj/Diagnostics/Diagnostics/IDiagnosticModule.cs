namespace Dragon.Diagnostics
{
    public interface IDiagnosticModule<in TOptions> where TOptions : DiagnosticsOptionsBase
    {
        string Execute(TOptions options);
    }
}
