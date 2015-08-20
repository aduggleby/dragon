using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dragon.Mail.Models
{
    public class MailAddressSerializerConverter : JsonConverter
    {
        public MailAddressSerializerConverter()
        {
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string displayName = string.Empty;
            string address = string.Empty;
            while (reader.Read())
            {
                var tokenType = reader.TokenType;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var val = (reader.Value as string )?? string.Empty;
                    if (val == "DisplayName")
                    {
                        displayName = reader.ReadAsString();
                    }
                    if (val == "Address")
                    {
                        address = reader.ReadAsString();
                    }
                }

                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            var mailAddress = new MailAddress(address, displayName);
            return mailAddress;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MailAddress);
        }
    }

}
