using System.Text.RegularExpressions;

namespace Dragon.Security.Hmac.Module.Services
{
    public class PathInfo
    {
        public enum PathType
        {
            Include,
            Exclude
        }
        public PathType Type { get; set; }
        public Regex Regex { get; set; }
    }
}
