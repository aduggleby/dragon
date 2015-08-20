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
    public class FileFolderTemplateRepository : IFileFolderTemplateRepository
    {
        public const string APP_KEY_FOLDER = "Dragon.Mail.Templates.Folder";
        public const string APP_KEY_DEFLANG = "Dragon.Mail.Templates.DefaultLanguage";

        public const string EXT_TEXT = ".txt";
        public const string EXT_HTML = ".html";

        public const string FILE_SUBJECT = "subject";
        public const string FILE_BODY = "body";
        public const string FILE_SUMMARYSUBJECT = "summarysubject";
        public const string FILE_SUMMARYBODY = "summarybody";
        public const string FILE_SUMMARYHEADER = "summaryheader";
        public const string FILE_SUMMARYFOOTER = "summaryfooter";

        private readonly IFileSystem m_fileSystem = null;
        private readonly IConfiguration m_configuration = null;
        private readonly string m_baseFolder;
        private readonly string m_ignorePrefix ;

        private readonly CultureInfo m_cultureInfo = CultureInfo.CurrentCulture;

        public FileFolderTemplateRepository(
            string directory = null,
            IConfiguration configuration = null,
            IFileSystem fileSystem = null,
            string ignorePrefix = "_")
        {
            m_ignorePrefix = ignorePrefix;
            m_fileSystem = fileSystem ?? new DefaultFileSystem();
            m_configuration = configuration ?? new DefaultConfiguration();
            m_baseFolder = (directory ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(m_baseFolder))
            {
                m_baseFolder = m_configuration.GetValue(APP_KEY_FOLDER);
            }

            if (string.IsNullOrWhiteSpace(m_baseFolder))
            {
                throw new ConfigurationMissingException(APP_KEY_FOLDER);
            }

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

            if (!m_fileSystem.ExistDir(m_baseFolder))
            {
                throw new Exception(string.Format("Template directory {0} does not exist.", m_baseFolder));
            }
        }

        public void EnumerateTemplates(Action<Template> act)
        {
            m_fileSystem.EnumerateDirectory(m_baseFolder, sub =>
            {
                if (sub.StartsWith(m_ignorePrefix))
                {
                    return;
                }

                var t = new Template();
                t.Key = sub.ToLower();
                var dir = Path.Combine(m_baseFolder, sub);
                FillTemplateContent(t.Content, dir);

                t.Language = m_cultureInfo;

                act(t);

                // languages
                m_fileSystem.EnumerateDirectory(dir, lang =>
                {
                    var langt = new Template();
                    langt.Key = sub.ToLower(); // use key from top folder

                    var langdir = Path.Combine(dir, lang);
                    FillTemplateContent(langt.Content, langdir);

                    try
                    {
                        var ci = CultureInfo.CreateSpecificCulture(lang.ToLower());
                        langt.Language = ci;
                    }
                    catch (CultureNotFoundException ex)
                    {
                        throw new Exception(string.Format("Path '{0}' ends in an invalid " +
                                                          "language specifier. It should " +
                                                          "be something like 'en' or 'en-us'.",
                            langdir));
                    }
                    act(langt);
                });
            });
        }

        private void FillTemplateContent(TemplateContent templateContent, string dir)
        {
            // subjects should not be html, but we will first try loading html file 
            if (m_fileSystem.ExistFile(Path.Combine(dir, FILE_SUBJECT + EXT_HTML)))
            {
                templateContent.Subject = m_fileSystem.GetContents(Path.Combine(dir, FILE_SUBJECT + EXT_HTML));
            }
            else
            {
                templateContent.Subject = m_fileSystem.GetContents(Path.Combine(dir, FILE_SUBJECT + EXT_TEXT));
            }

            if (m_fileSystem.ExistFile(Path.Combine(dir, FILE_SUMMARYSUBJECT + EXT_HTML)))
            {
                templateContent.SummarySubject = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYSUBJECT + EXT_HTML));
            }
            else
            {
                templateContent.SummarySubject = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYSUBJECT + EXT_TEXT));
            }

            templateContent.Body = GetContentsExpandIncludes(Path.Combine(dir, FILE_BODY + EXT_HTML));
            templateContent.SummaryBody = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYBODY + EXT_HTML));
            templateContent.SummaryHeader = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYHEADER + EXT_HTML));
            templateContent.SummaryFooter = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYFOOTER + EXT_HTML));

            templateContent.TextBody = GetContentsExpandIncludes(Path.Combine(dir, FILE_BODY + EXT_TEXT));
            templateContent.SummaryTextBody = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYBODY + EXT_TEXT));
            templateContent.SummaryTextHeader = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYHEADER + EXT_TEXT));
            templateContent.SummaryTextFooter = GetContentsExpandIncludes(Path.Combine(dir, FILE_SUMMARYFOOTER + EXT_TEXT));
        }

        private string GetContentsExpandIncludes(string file)
        {
            var content = m_fileSystem.GetContents(file);

            if (string.IsNullOrWhiteSpace(content)) return null;

            return Regex.Replace(content, @"@inc\(([a-zA-Z0-9\./\\_-]*)\)", (m) =>
            {
                var filename = m.Groups[1].Value;
                var incfile = Path.Combine(Path.GetDirectoryName(file), filename);
                if (!m_fileSystem.ExistFile(incfile))
                {
                    return "File " + incfile + " not found.";
                }
                else
                {
                    return GetContentsExpandIncludes(incfile);
                }
            });
            
        }
    }
}
