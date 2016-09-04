using System.Collections.Generic;

namespace Dragon.SecurityServer.GenericSTSClient
{
    public interface IHmacHelper
    {
        string CalculateHash(Dictionary<string, string> parameters, string secret);
        Dictionary<string, string> CreateHmacRequestParametersFromConfig();
        Dictionary<string, string> CreateHmacRequestParametersFromConfig(Dictionary<string, string> parameters);
        Dictionary<string, string> CreateHmacRequestParametersFromConfig(string settingsPrefix);
        Dictionary<string, string> CreateHmacRequestParametersFromConfig(Dictionary<string, string> parameters, string settingsPrefix);
    }
}
