using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;

namespace Dragon.Mail.Impl
{
    public class RestResolvingDecorator : IDataDecorator
    {
        private IHttpClient m_httpClient;
        private IDataStore m_dataStore;

        public RestResolvingDecorator(IHttpClient httpClient, IDataStore dataStore = null)
        {
            m_httpClient = httpClient;
            m_dataStore = dataStore;
        }

        public dynamic Decorate(dynamic data)
        {
            var resolvedData = new ExpandoObject() as IDictionary<string, Object>;

            if (data == null) return resolvedData;

            if (data is Uri)
            {
                return GetCachedOfFetchUri((Uri)data);
            }
            else if (data is IDictionary<string, object>)
            {
                var dataDictionary = (IDictionary<string, object>)data;

                foreach (var key in dataDictionary.Keys)
                {
                    var val = dataDictionary[key];
                    if (val is Uri)
                    {
                        val = GetCachedOfFetchUri((Uri)val);
                    }
                    resolvedData.Add(key, val);
                }
            }
            else
            {
                foreach (var prop in data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var val = prop.GetValue(data, null);
                    if (val is Uri)
                    {
                        val = GetCachedOfFetchUri((Uri)val);
                    }
                    resolvedData.Add(prop.Name, val);
                }
            }

            return (dynamic)resolvedData;
        }

        private dynamic GetCachedOfFetchUri(Uri uri)
        {
            dynamic cached = null;

            if (m_dataStore != null)
            {
                cached = m_dataStore.Get(uri);
            }
            if (cached == null)
            {
                var task = m_httpClient.GetAsync(uri);
                task.Wait();
                cached = task.Result;
                if (m_dataStore != null)
                {
                    m_dataStore.Set(uri, cached);
                }
            }
            return cached;
        }
    }
}
