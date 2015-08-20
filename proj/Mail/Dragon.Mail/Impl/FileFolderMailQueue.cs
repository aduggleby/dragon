using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Utils;
using Newtonsoft.Json;

namespace Dragon.Mail.Impl
{
    public class FileFolderMailQueue : IMailQueue, IDisposable
    {
        public const string APP_FOLDER = "Dragon.Mail.FileFolderMailQueue.Folder";
        public const string FOLDER_QUEUE = "queue";
        public const string FOLDER_SENT = "sent";
        public const string FOLDER_ERROR = "error";

        private string m_baseDirectory;
        private string m_queueDir;
        private string m_errorDir;
        private string m_sentDir;
        private string m_workerDir;

        private JsonSerializerSettings m_settings;
        private IConfiguration m_configuration;
        private IFileSystem m_fileSystem;

        public FileFolderMailQueue(
            IFileSystem fileSystem = null,
            IConfiguration configuration = null)
        {
            m_settings = new JsonSerializerSettings();
            m_settings.TypeNameHandling = TypeNameHandling.All;
            m_settings.NullValueHandling = NullValueHandling.Ignore;

            m_configuration = configuration ?? new DefaultConfiguration();

            m_fileSystem = fileSystem ?? new DefaultFileSystem();


            m_baseDirectory = m_configuration.GetValue(APP_FOLDER);
            if (string.IsNullOrWhiteSpace(m_baseDirectory))
            {
                throw new ConfigurationMissingException(APP_FOLDER);
            }

            if (!m_fileSystem.ExistDir(m_baseDirectory))
            {
                throw new Exception(string.Format("Directory '{0}' does not exist. Cannot continue.",
                    m_baseDirectory));
            }

            m_fileSystem.CreateDir(m_queueDir = Path.Combine(m_baseDirectory, FOLDER_QUEUE));
            m_fileSystem.CreateDir(m_sentDir = Path.Combine(m_baseDirectory, FOLDER_SENT));
            m_fileSystem.CreateDir(m_errorDir = Path.Combine(m_baseDirectory, FOLDER_ERROR));
            m_fileSystem.CreateDir(m_workerDir = Path.Combine(m_baseDirectory, "worker-" + Guid.NewGuid().ToString()));

        }

        public void Enqueue(Models.Mail mail, dynamic additionalParameters)
        {
            var serialized = JsonConvert.SerializeObject(mail, m_settings);

            int retry = 0;
            bool success = false;
            string lastFileName = string.Empty;
            while (retry++ < 3 && !success)
            {
                success = m_fileSystem.Save(lastFileName = Path.Combine(m_queueDir, "mail-" + Guid.NewGuid().ToString()), serialized);
            }

            if (!success)
            {
                throw new Exception(string.Format("Could not save file: {0}.", lastFileName));
            }
        }

        public bool Dequeue(Func<Models.RenderedMail, bool> processor)
        {
            return m_fileSystem.MoveOldestToDir(
                m_queueDir,
                m_workerDir,
                m_sentDir,
                m_errorDir,
                (contents) =>
                {
                    var deserialized = JsonConvert.DeserializeObject<Models.Mail>(contents, m_settings);

                    var rendered = new RenderedMail();
                    rendered.Receiver = deserialized.Receiver;
                    rendered.Sender = deserialized.Sender;
                    rendered.Subject = deserialized.Subject;
                    rendered.Body = deserialized.Body;
                    rendered.TextBody = deserialized.TextBody;
                    
                    return processor(rendered);
                });
        }

        public void Dispose()
        {
            try
            {
                m_fileSystem.DeleteDirectoryIfEmpty(m_workerDir);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
