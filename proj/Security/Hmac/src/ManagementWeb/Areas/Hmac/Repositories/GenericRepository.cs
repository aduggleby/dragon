using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ManagementWeb.Areas.Hmac.Models;
using Newtonsoft.Json;

namespace ManagementWeb.Areas.Hmac.Repositories
{
    public class GenericRepository<TModel, TKey> : IGenericRepository<TModel, TKey>
        where TModel : IModel<TKey>
    {
        public string ServiceUrl { get; set; }

        public async Task<IList<TModel>> List()
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(ServiceUrl);
                return JsonConvert.DeserializeObject<List<TModel>>(response);
            }
        }

        public async Task<string> Add(TModel model)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.PostAsJsonAsync(ServiceUrl, model);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
                return result;
            }
        }

        public async Task<TModel> Details(TKey id)
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(ServiceUrl + "/" + id);
                return JsonConvert.DeserializeObject<TModel>(response);
            }
        }

        public async Task Edit(TModel model)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.PutAsJsonAsync(ServiceUrl + "/" + model.Id, model);
                if (!response.IsSuccessStatusCode)
                {
                    throw new IOException(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task Delete(TKey id)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.DeleteAsync(ServiceUrl + "/" + id);
                if (!response.IsSuccessStatusCode)
                {
                    throw new IOException(await response.Content.ReadAsStringAsync());
                }
            }
        }

        # region helper

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        # endregion
    }
}