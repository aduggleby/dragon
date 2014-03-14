using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Antlr4.StringTemplate;
using Dragon.Interfaces;
using Dragon.Interfaces.ActivityCenter;
using System.Linq;
using Dragon.Interfaces.Core;
using StructureMap;

namespace Dragon.Core.Mail
{
    public class Antlr4StringTemplateService : IEmailTemplateService
    {
        private string m_commonTemplateDirectory;
        private readonly IDirectoryService m_dirService;
        private readonly IFileService m_fileService;

        private const string CONFIG_BASEDIR = "Dragon.Templates.StringTemplate.BaseDir";
        
        [DefaultConstructor]
        public Antlr4StringTemplateService(IConfiguration configuration, IDirectoryService dirService, IFileService fileService)
        {
            m_dirService = dirService;
            m_fileService = fileService;
            var baseDir = configuration.GetValue(CONFIG_BASEDIR, "~/templates");

            ParseBaseDir(baseDir);

            EnsureBaseDirExists();
        }


        public Antlr4StringTemplateService(string baseDir, IDirectoryService dirService, IFileService fileService)
        {
            m_dirService = dirService;
            m_fileService = fileService;

            ParseBaseDir(baseDir);

            EnsureBaseDirExists();
        }

        public ITemplateServiceResult Generate(string type, string[] subtypeOrder, string culture, Dictionary<string,object> model)
        {
            culture = culture ?? "en-us";

            var result = new TemplateServiceResult();

            var searchedTemplateNames = new List<string>();
            var templateName = string.Empty;
            var found = false;
            foreach (var subtype in subtypeOrder)
            {
                templateName = string.Format("{0}_{1}_{2}.st",
                    type,
                    culture,
                    subtype);

                var searchfor = Path.Combine(m_commonTemplateDirectory, templateName);
                searchedTemplateNames.Add(searchfor);

                if (m_fileService.Exists(searchfor))
                {
                    found = true;
                    result.Subtype = subtype;
                    break;
                }
            }

            if (!found)
            {
                throw new Exception("Template not found. Searched for: " + string.Join(", ", searchedTemplateNames.ToArray()));
            }

            var file = m_fileService.GetFileContents(Path.Combine(m_commonTemplateDirectory, templateName));
            var subject = (string)null;
            const string SUBJECT_PREFIX = "Subject:";
            if (file.Any() && file[0].ToUpper().StartsWith(SUBJECT_PREFIX.ToUpper()))
            {
                subject = file.First().Substring(SUBJECT_PREFIX.Length).Trim();
                file = file.Skip(1).ToArray();
            }

            var tmpl = new Template(string.Join(Environment.NewLine, file), '$', '$');
            foreach (var key in model.Keys)
            {
                tmpl.Add(key, model[key]);
            }

            result.Body = tmpl.Render();

            if (subject != null)
            {
                var subjecttmpl = new Template(subject, '$', '$');
                foreach (var key in model.Keys)
                {
                    subjecttmpl.Add(key, model[key]);
                }

                result.Subject = subjecttmpl.Render();
            }

            return result;
        }


        public ITemplateServiceResult Generate(IEnumerable<IActivity> activity, string[] subtypeOrder, INotifiable notifiable)
        {
            if (!activity.Any()) throw new Exception("No activities to process.");

            if (!activity.All(x => x.GetType() == activity.First().GetType()))
                throw new Exception("Activities must all have the same type.");

            var result = new TemplateServiceResult();

            var searchedTemplateNames = new List<string>();
            var templateName = string.Empty;
            var found = false;
            foreach (var subtype in subtypeOrder)
            {
                var type = activity.First().GetType().Name;

                templateName = string.Format("{0}_{1}_{2}.st",
                    type,
                    notifiable.PrimaryCulture.Name,
                    subtype);


                searchedTemplateNames.Add(templateName);

                if (m_fileService.Exists(Path.Combine(m_commonTemplateDirectory, templateName)))
                {
                    found = true;
                    result.Subtype = subtype;
                    break;
                }
            }

            if (!found)
            {
                throw new Exception("Template not found. Searched for: " + string.Join(",", searchedTemplateNames.ToArray()));
            }

            var file = m_fileService.GetFileContents(Path.Combine(m_commonTemplateDirectory, templateName));
            var subject = (string)null;
            const string SUBJECT_PREFIX = "Subject:";
            if (file.Any() && file[0].ToUpper().StartsWith(SUBJECT_PREFIX.ToUpper()))
            {
                subject = file.First().Substring(SUBJECT_PREFIX.Length).Trim();
                file = file.Skip(1).ToArray();
            }

            
            var list = activity.ToList();
            var first = list.FirstOrDefault();

            var tmpl = new Template(string.Join(Environment.NewLine, file), '$', '$');
            tmpl.Add("models", list);
            tmpl.Add("model", first);

            tmpl.Add("to", notifiable);
            result.Body = tmpl.Render();

            if (subject != null)
            {
                var subjecttmpl = new Template(subject, '$', '$');
                subjecttmpl.Add("models", list);
                subjecttmpl.Add("model", first);
                subjecttmpl.Add("to", notifiable);
                result.Subject = subjecttmpl.Render();
            }

            //foreach (var p in parameter)
            //{
            //    if (p.Value is string || !(p.Value is IEnumerable))
            //    {
            //        tmpl.Add(p.Key, p.Value);
            //    }
            //    else
            //    {
            //        tmpl.AddMany(p.Key, ((IEnumerable)p.Value).Cast<object>().ToArray());
            //    }
            //}

            return result;
        }


        private void ParseBaseDir(string baseDir)
        {
            if (baseDir.StartsWith("~"))
            {
                var ctx = HttpContext.Current;
                if (ctx == null)
                    throw new Exception(
                        string.Format(
                            "Configuration value {0} starts with ~ but you are not running in an HttpContext.",
                            CONFIG_BASEDIR));

                m_commonTemplateDirectory = ctx.Server.MapPath(baseDir);
            }
            else
            {
                m_commonTemplateDirectory = baseDir;
            }
        }


        private void EnsureBaseDirExists()
        {
            if (!m_dirService.Exists(m_commonTemplateDirectory))
                throw new Exception(
                    string.Format(
                        "Base directory '{0}' points to a non existant directory.",
                        m_commonTemplateDirectory));
        }


    }
}
