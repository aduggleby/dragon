using System.Collections.Generic;
using System.Configuration;

namespace Dragon.Security.Hmac.Module.Configuration
{
    public class PathCollection : ConfigurationElementCollection, IEnumerable<PathConfig>
    {

        public PathConfig this[int index]
        {
            get { return (PathConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(PathConfig serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PathConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PathConfig)element).Name;
        }

        public void Remove(PathConfig serviceConfig)
        {
            BaseRemove(serviceConfig.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public new IEnumerator<PathConfig> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return BaseGet(i) as PathConfig;
            }
        }
    }
}
