using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Utils;

namespace Dragon.Mail.Impl
{

    public class DefaultFileSystem : IFileSystem
    {
        private IFileSystemEnumerator m_fileEnumerator;
        private Encoding m_encoding;

        public DefaultFileSystem(IFileSystemEnumerator fileEnumerator = null,
            Encoding encoding = null)
        {
            m_fileEnumerator = fileEnumerator ?? new FastFileSystemEnumerator();
            m_encoding = encoding ?? Encoding.UTF8;
        }

        public bool CreateDir(string dir, bool ignoreIfExists = true)
        {
            if (ignoreIfExists && ExistDir(dir)) return false;

            var di = new DirectoryInfo(dir);
            di.Create();
            return true;
        }

        public bool ExistDir(string dir)
        {
            var di = new DirectoryInfo(dir);
            return di.Exists;
        }

   

        public void EnumerateDirectory(string dir, Action<string> act)
        {
            var di = new DirectoryInfo(dir);
            if (!di.Exists)
            {
                throw new Exception("Directory does not exist.");
            }

            di.GetDirectories().ToList().ForEach(sub => act(sub.Name));
        }

        public string GetContents(string file, bool failIfNotExists = false)
        {
            if (!File.Exists(file))
            {
                if (failIfNotExists)
                {
                    throw new Exception(string.Format("Expected file '{0}' not found.", file));
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return File.ReadAllText(file, m_encoding);
            }
        }

        public string PeekOldest(string sourceDir)
        {
            var diSource = new DirectoryInfo(sourceDir);
            if (!diSource.Exists)
            {
                throw new ArgumentException("Directory does not exist.", "sourceDir");
            }
          
            // Find oldest
            var path = m_fileEnumerator
                .EnumerateFiles(diSource)
                .OrderBy(x => x.CreationTimeUtc)
                .FirstOrDefault();

            if (path == null) return null;

            return File.ReadAllText(path.Path);

        }

        public bool MoveOldestToDir(
            string sourceDir,
            string workerDir,
            string successDir,
            string failureDir, 
            Func<string,bool> processContents)
        {
            var diSource = new DirectoryInfo(sourceDir);
            if (!diSource.Exists)
            {
                throw new ArgumentException("Directory does not exist.", "sourceDir");
            }
            var diSuccess = new DirectoryInfo(successDir);
            if (!diSuccess.Exists)
            {
                throw new ArgumentException("Directory does not exist.", "diSuccess");
            }
            var diFailure = new DirectoryInfo(failureDir);
            if (!diFailure.Exists)
            {
                throw new ArgumentException("Directory does not exist.", "diFailure");
            }
             var diWorker = new DirectoryInfo(workerDir);
            if (!diWorker.Exists)
            {
                throw new ArgumentException("Directory does not exist.", "diWorker");
            }

            // Find oldest
            var path = m_fileEnumerator
                .EnumerateFiles(diSource)
                .OrderBy(x => x.CreationTimeUtc)
                .FirstOrDefault();

            if (path == null)
            {
                return false;
            }

            // Move to worker directory during processing
            if (!Move(path.Path, diWorker.FullName))
            {
                return false; // concurrent access happened
            }

            var newpath = Path.Combine(diWorker.FullName, Path.GetFileName(path.Path));

            // Read contents and allow for processing, move appropriately
            var contents = File.ReadAllText(newpath, m_encoding);

            if (processContents(contents))
            {
                if (!Move(newpath, diSuccess.FullName))
                {
                    throw new Exception(
                        "Could not move file from worker directory. Probably concurrent use of the same worker directory.");
                }
            }
            else
            {
                if (!Move(newpath, diFailure.FullName))
                {
                    throw new Exception(
                        "Could not move file from worker directory. Probably concurrent use of the same worker directory.");
                }
            }

            return true;
        }

        public void DeleteDirectoryIfEmpty(string dir)
        {
            var di = new DirectoryInfo(dir);
            if (di.Exists && !di.GetFiles().Any()) di.Delete();
        }

        public bool ExistFile(string file)
        {
            var fi = new FileInfo(file);
            return fi.Exists;
        }

        public void Remove(string file)
        {
            var fi = new FileInfo(file);
            if (fi.Exists) fi.Delete();
        }

        public bool Move(string file, string newdir)
        {
            var fi = new FileInfo(file);
            if (fi.Exists)
            {
                fi.MoveTo(Path.Combine(newdir, fi.Name));
                return true;
            }
            return false;
        }

        public bool Save(string file, string contents, bool overwrite = false)
        {
            var fi = new FileInfo(file);
            var exists = ExistFile(fi.FullName);
            if (!exists || overwrite)
            {
                File.WriteAllText(fi.FullName, contents, m_encoding);
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
