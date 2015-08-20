using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Interfaces
{
    public interface IHttpClient
    {
        Task<dynamic> GetAsync(Uri uri);
    }
}
