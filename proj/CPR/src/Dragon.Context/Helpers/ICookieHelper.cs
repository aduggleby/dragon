namespace Dragon.Context.Helpers
{
    public interface ICookieHelper
    {
        void Add(string key, string value);
        string Get(string key);
    }
}
