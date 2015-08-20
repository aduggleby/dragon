using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Models
{
    public class Template
    {
        public Template()
        {
            Content = new TemplateContent();
            Language = CultureInfo.CurrentCulture;
        }

        public string Key { get; set; }

        public CultureInfo Language { get; set; }

        public TemplateContent Content { get; set; }

        public string LocalizedKey
        {
            get { return GenerateLocalizedKey(Key, Language); }
        }

        public static string GenerateLocalizedKey(string key, CultureInfo language)
        {
            var langKey = language.Name.ToLower();
            var localizedTemplateKey = string.Format("{0}-{1}", key, langKey);
            return localizedTemplateKey;
        }
    }

    public class TemplateContent
    {
        private string m_summarySubject;
        private string m_subject;

        public string Subject
        {
            get { return m_subject; }
            set
            {
                value = value ?? string.Empty;
                if (value.Contains('\r') || value.Contains('\n'))
                    throw new Exception("Subjects cannot contain new line characters.");

                m_subject = value;
            }
        }

        public string Body { get; set; }
        public string TextBody { get; set; }

        public string SummarySubject
        {
            get { return m_summarySubject; }
            set
            {
                value = value ?? string.Empty;
                if (value.Contains('\r') || value.Contains('\n'))
                    throw new Exception("Subjects cannot contain new line characters.");

                m_summarySubject = value;
            }
        }



        public string SummaryHeader { get; set; }

        public string SummaryBody { get; set; }

        public string SummaryFooter { get; set; }

        public string SummaryTextHeader { get; set; }

        public string SummaryTextBody { get; set; }

        public string SummaryTextFooter { get; set; }
    }
}
