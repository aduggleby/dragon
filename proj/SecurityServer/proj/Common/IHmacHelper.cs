using System.Collections.Generic;

namespace Dragon.SecurityServer.Common
{
    public interface IHmacHelper
    {
        Dictionary<string, string> CreateHmacRequestParametersFromConfig();
    }
}