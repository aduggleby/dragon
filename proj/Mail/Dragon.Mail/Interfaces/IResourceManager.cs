using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Interfaces
{
    public interface IResourceManager
    {
        IEnumerable<CultureInfo> GetAvailableCultures();
        IEnumerable<string> GetKeys(CultureInfo ci = null);
        string GetString(string key, CultureInfo ci = null);
    }
}
