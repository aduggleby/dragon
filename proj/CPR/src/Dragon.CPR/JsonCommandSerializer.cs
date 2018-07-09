using Dragon.CPR.Interfaces;
using Newtonsoft.Json;

namespace Dragon.CPR
{
    public class JsonCommandSerializer : ICommandSerializer
    {
        private readonly JsonSerializerSettings m_jsonSerializerSettings;

        public JsonCommandSerializer()
        {
            m_jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public virtual string Serialize(CommandBase command)
        {
            return JsonConvert.SerializeObject(command, m_jsonSerializerSettings);
        }

        public virtual object Deserialize(Command command)
        {
            return JsonConvert.DeserializeObject(command.JSON, m_jsonSerializerSettings);
        }
    }
}
