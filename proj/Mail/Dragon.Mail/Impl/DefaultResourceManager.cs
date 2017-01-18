using Dragon.Mail.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Impl
{
    public class DefaultResourceManagerAdapter : IResourceManager
    {
        private ResourceManager m_resMgr;

        private IEnumerable<CultureInfo> m_availableCultures;

        public DefaultResourceManagerAdapter(ResourceManager resMgr)
        {
            m_resMgr = resMgr;

            EnumerateAvailableCultures();
        }

        public IEnumerable<string> GetKeys(CultureInfo ci = null)
        {
            ci = ci ?? CultureInfo.InvariantCulture;

            var entries = m_resMgr.GetResourceSet(ci, true, false);

            if (entries == null) yield break;

            foreach (DictionaryEntry entry in entries)
            {
                if (!(entry.Key is string))
                {
                    Trace.TraceWarning($"There are non string keys in the resource set {m_resMgr.BaseName} for culture {ci.Name}. These will not be enumerated for use by Dragon.Mail.");
                }
                else
                {
                    yield return entry.Key as string;
                }
            }
        }

        public IEnumerable<CultureInfo> GetAvailableCultures()
        {
            return m_availableCultures;
        }

        /// <summary>
        /// This will test all available cultures in .NET against the resource manager.
        /// This seems very expensive, so we do it in ctor.
        /// </summary>
        private void EnumerateAvailableCultures()
        {
            var availableCultures = new List<CultureInfo>();

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                try
                {
                    var rs = m_resMgr.GetResourceSet(culture, true, false);
                    if (rs != null)
                    {
                        availableCultures.Add(culture);
                    }
                }
                catch (CultureNotFoundException)
                {
                    // ignore it
                }
            }

            m_availableCultures = availableCultures.ToArray();
        }

        public string GetString(string key, CultureInfo ci = null)
        {
            ci = ci ?? CultureInfo.InvariantCulture;
            
            return m_resMgr.GetString(key, ci);
        }
    }
}
