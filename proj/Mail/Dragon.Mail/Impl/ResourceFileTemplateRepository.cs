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
using System.Resources;
using System.Diagnostics;

namespace Dragon.Mail.Impl
{
    public class ResourceFileTemplateRepository : TemplateRepositoryBase, ITemplateRepository
    {
        public const string KEY_SUBJECT = "{0}_subject_{1}";
        public const string KEY_BODY = "{0}_body_{1}";
        public const string KEY_SUMMARYSUBJECT = "{0}_summarysubject_{1}";
        public const string KEY_SUMMARYBODY = "{0}_summarybody_{1}";
        public const string KEY_SUMMARYHEADER = "{0}_summaryheader_{1}";
        public const string KEY_SUMMARYFOOTER = "{0}_summaryfooter_{1}";

        public const string REGEX_EXTRACT_KEY = @"^([\w^_]+)_[\w^_]+_[^_]+$";

        public const string KEY_TYPE_HTML = "html";
        public const string KEY_TYPE_TEXT = "text";

        private readonly IResourceManager m_resMgr;

        private readonly string m_ignorePrefix;

        public ResourceFileTemplateRepository(
            IResourceManager resMgr,
            IConfiguration configuration = null,
            string ignorePrefix = "_") : base(configuration)
        {
            m_ignorePrefix = ignorePrefix;

            if (resMgr == null)
            {
                throw new ArgumentException("Resource Manager is required.", "resMgr");
            }

            m_resMgr = resMgr;
        }

        public override void EnumerateTemplates(Action<Template> act)
        {
            var cultures = m_resMgr.GetAvailableCultures().ToArray();
            var mainKeys = m_resMgr.GetKeys().Where(x => !x.StartsWith(m_ignorePrefix)).ToArray();
            var handled = new List<string>();

            foreach (var _culture in cultures)
            {
                var culture = _culture;

                if (culture.Equals(CultureInfo.InvariantCulture))
                {
                    culture = m_cultureInfo; // set to default culture
                }

                foreach (var key in mainKeys)
                {
                    var t = new Template();

                    var match = Regex.Match(key, REGEX_EXTRACT_KEY);
                    if (match == null)
                    {
                        Trace.TraceWarning("Key in resource manager did not match the expected string: " + REGEX_EXTRACT_KEY);
                        continue;
                    }

                    var templateName = match.Groups[1].Value.Trim();

                    var localizedTemplateName = templateName + "-" + culture.ToString();
                    if (handled.Contains(localizedTemplateName)) continue; // we will the same multiple times for subject, body, etc.
                    handled.Add(localizedTemplateName);

                    t.Key = templateName;
                    t.Language = culture;
                    FillTemplateContent(t.Content, culture, t.Key);

                    act(t);
                }
            }
        }

        private void FillTemplateContent(TemplateContent templateContent, CultureInfo ci, string key)
        {
            // subjects should not be html, but we will first try loading the html key and then fallback to the text key
            templateContent.Subject =
                GetContentsExpandIncludes(string.Format(KEY_SUBJECT, key, KEY_TYPE_HTML), ci) ??
                    GetContentsExpandIncludes(string.Format(KEY_SUBJECT, key, KEY_TYPE_TEXT), ci);

            templateContent.SummarySubject =
                GetContentsExpandIncludes(string.Format(KEY_SUMMARYSUBJECT, key, KEY_TYPE_HTML), ci) ??
                    GetContentsExpandIncludes(string.Format(KEY_SUMMARYSUBJECT, key, KEY_TYPE_TEXT), ci);

            templateContent.Body = GetContentsExpandIncludes(string.Format(KEY_BODY, key, KEY_TYPE_HTML), ci);
            templateContent.SummaryBody = GetContentsExpandIncludes(string.Format(KEY_SUMMARYBODY, key, KEY_TYPE_HTML), ci);
            templateContent.SummaryHeader = GetContentsExpandIncludes(string.Format(KEY_SUMMARYHEADER, key, KEY_TYPE_HTML), ci);
            templateContent.SummaryFooter = GetContentsExpandIncludes(string.Format(KEY_SUMMARYFOOTER, key, KEY_TYPE_HTML), ci);

            templateContent.TextBody = GetContentsExpandIncludes(string.Format(KEY_BODY, key, KEY_TYPE_TEXT), ci);
            templateContent.SummaryTextBody = GetContentsExpandIncludes(string.Format(KEY_SUMMARYBODY, key, KEY_TYPE_TEXT), ci);
            templateContent.SummaryTextHeader = GetContentsExpandIncludes(string.Format(KEY_SUMMARYHEADER, key, KEY_TYPE_TEXT), ci);
            templateContent.SummaryTextFooter = GetContentsExpandIncludes(string.Format(KEY_SUMMARYFOOTER, key, KEY_TYPE_TEXT), ci);
        }

        private string GetContentsExpandIncludes(string key, CultureInfo ci)
        {
            var content = m_resMgr.GetString(key, ci);
            if (content == null) return null;

            return Regex.Replace(content, @"@inc\(([a-zA-Z0-9\./\\_-]*)\)", (m) =>
            {
                var includeKey = m.Groups[1].Value;

                var includeContent = GetContentsExpandIncludes(includeKey, ci);

                if (includeContent == null) // we don't want to check for empty strings, since that may be a valid replace
                {
                    return $"Resource entry for {includeKey} not found.";
                }
                else
                {
                    return includeContent;
                }
            });

        }
    }
}
