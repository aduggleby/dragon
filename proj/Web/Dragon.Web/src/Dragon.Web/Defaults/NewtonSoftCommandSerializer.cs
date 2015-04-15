using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Web.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dragon.Web.Defaults
{
    public class NewtonsoftCommandSerializer : ICommandSerializer
    {
        private readonly JsonSerializerSettings m_jsonSerializerSettings;

        public NewtonsoftCommandSerializer()
        {
            m_jsonSerializerSettings = new JsonSerializerSettings();

            m_jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            m_jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
            m_jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter());
        }

        public string Serialize(object command)
        {
            return JsonConvert.SerializeObject(command, m_jsonSerializerSettings);
        }
    }
}
