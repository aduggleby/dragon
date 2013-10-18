using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Dragon.Interfaces;
using Dragon.Interfaces.ActivityCenter;

namespace Dragon.Tests.Core.Mail.StringTemplateService
{
    public class _base: IDirectoryService, IFileService
    {
        protected Func<string, string> m_fileContents;
        protected Func<string, bool> m_existsFile;
        protected Func<string, bool> m_existsDir;

        protected string[] SUBTYPE_TEXT = new string[] {"text"};
        protected string[] SUBTYPE_HTML_TEXT = new string[] { "html", "text" };
        
        public string[] GetFileContents(string path)
        {
            return m_fileContents(path).Split(Environment.NewLine);
        }

        bool IFileService.Exists(string path)
        {
            return m_existsFile(path);
        }

        bool IDirectoryService.Exists(string directory)
        {
            return m_existsDir(directory);
        }
    }

    public class MockActivity : IActivity
    {
        public string Name { get; set; }
    }

    public class MockActivity2 : IActivity
    {

    }

    public class MockNotifiable : INotifiable
    {
        public CultureInfo PrimaryCulture { get; set; }
        public string Name { get; set; }
    }

}
