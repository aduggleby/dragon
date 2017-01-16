using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Utils;

namespace Dragon.Mail.Impl
{
    public abstract class TemplateRepositoryBase 
    {
        public const string APP_KEY_DEFLANG = "Dragon.Mail.Templates.DefaultLanguage";

        protected readonly IConfiguration m_configuration = null;

        protected readonly CultureInfo m_cultureInfo = CultureInfo.CurrentCulture;

        public TemplateRepositoryBase(
            IConfiguration configuration = null)
        {
            m_configuration = configuration ?? new DefaultConfiguration();
            
            var culture = m_configuration.GetValue(APP_KEY_DEFLANG);
            if (!string.IsNullOrWhiteSpace(culture))
            {
                try
                {
                    m_cultureInfo = CultureInfo.CreateSpecificCulture(culture);
                }
                catch (CultureNotFoundException)
                {
                    throw new Exception(string.Format("The specified default culture is " +
                                                      "{0} is invalid. Use format such as " +
                                                      "en and en-us.",
                        culture));
                }
            }

        }

        public abstract void EnumerateTemplates(Action<Template> act);
    }
}
