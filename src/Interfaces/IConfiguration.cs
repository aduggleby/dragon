using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces
{
    public interface IConfiguration
    {
        T GetValue<T>(string configKey, T defaultValue);
        T EnsureValue<T>(string configKey);
    }
}
