using Division42.NetworkTools.TraceRoute;

namespace Dragon.Diagnostics.Modules
{
    public class TraceRouteModule : DiagnosticsModuleBase<TraceRouteOptions>
    {
        protected override void ExecuteImpl(TraceRouteOptions options)
        {
            var instance = new TraceRouteManager();
            DebugMessage(TraceRouteHopDetail.FormattedTextHeader);
            instance.TraceRouteNodeFound += (sender, e) =>
            {
                DebugMessage(e.Detail.ToString());
            };
            instance.TraceRouteComplete += (sender, e) =>
            {
                DebugMessage("Trace complete.");
            };
            var results = instance.ExecuteTraceRoute(options.Host);
            results.Wait();
        }
    }
}
