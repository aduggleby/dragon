using System.Collections.Generic;

namespace Dragon.SecurityServer.GenericSTSClient
{
    public interface IHmacHelper
    {
        Dictionary<string, string> CreateHmacRequestParametersFromConfig();
    }
}