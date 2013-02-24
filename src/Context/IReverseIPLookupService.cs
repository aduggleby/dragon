namespace Dragon.Context.ReverseIPLookup
{
    public interface IReverseIPLookupService
    {
        string GetLocationString(string ipAddress);
    }
}
