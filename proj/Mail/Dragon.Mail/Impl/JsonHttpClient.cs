using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Dragon.Mail.Interfaces;
using WebAPI = System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Impl
{
    public class JsonHttpClient : IHttpClient
    {
        private WebAPI.HttpClient m_httpClient;

        public JsonHttpClient()
        {
            m_httpClient = new WebAPI.HttpClient();
        }

        public async Task<dynamic> GetAsync(Uri uri)
        {
            var task = await m_httpClient.GetAsync(uri);
            return await task.Content.ReadAsAsync<dynamic>();
        }

    }
}
