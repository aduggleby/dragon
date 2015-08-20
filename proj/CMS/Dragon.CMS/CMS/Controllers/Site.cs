using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Dragon.CMS.CMS.Controllers
{
    public class Site
    {
        public static Site Load()
        {
            var path = HttpContext.Current.Server.MapPath("~/pages/site.json");
            return JsonConvert.DeserializeObject<Site>(File.ReadAllText(path));
        }

        [JsonProperty("testing")]
        public bool DebugMode { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("navigation")]
        [JsonConverter(typeof(NavigationConverter))]
        public Navigation Navigation { get; set; }
    }

    public class NavigationConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var nav = new Stack<NavigationItems>();
            var baseItem = new NavigationItems();
            nav.Push(baseItem);

            string name = string.Empty;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    if (nav.Count==1)
                    {
                        break;
                    }

                    nav.Pop();
                    continue;
                }

                // read display text
                var tokenType = reader.TokenType;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    name = (reader.Value as string) ?? string.Empty;
                }

                // progress to link or collection
                reader.Read();
                if (reader.TokenType == JsonToken.StartObject)
                {
                    var coll = new NavigationItems();
                    coll.DisplayText = name;
                    nav.First().Items.Add(coll);
                    nav.Push(coll);
                }
                else if (reader.TokenType == JsonToken.String)
                {
                    var item = new NavigationItem();
                    item.DisplayText = name;
                    item.Url = new Uri(reader.Value as string, UriKind.RelativeOrAbsolute);
                    nav.First().Items.Add(item);
                }
            }

            return baseItem;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Navigation);
        }
    }

    public abstract class Navigation
    {
        public string DisplayText;
    }

    public class NavigationItems : Navigation
    {
        public List<Navigation> Items = new List<Navigation>();
    }

    public class NavigationItem : Navigation
    {
        public Uri Url;
    }

    public class This
    {
        private static Site m_site;

        public static Site Site
        {
            get
            {
                if (m_site == null)
                {
                    var site = Site.Load();
                    if (!site.DebugMode)
                    {
                        m_site = site;
                    }
                    else
                    {
                        return site;
                    }
                }
                return m_site;
            }
        }
    }
}