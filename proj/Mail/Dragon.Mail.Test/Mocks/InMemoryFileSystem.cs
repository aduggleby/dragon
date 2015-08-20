using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Interfaces;

namespace Dragon.Mail.Test.Mocks
{
    public class InMemoryFileSystem : IFileSystem
    {
        private Dictionary<string, string> m_files = new Dictionary<string, string>();

        private IFileSystem m_baseFileSystemMock;

        public InMemoryFileSystem(IFileSystem baseFileSystemMock)
        {
            m_baseFileSystemMock = baseFileSystemMock;
        }

        public bool CreateDir(string dir, bool ignoreIfExists = true)
        {
            return m_baseFileSystemMock.CreateDir(dir, ignoreIfExists);
        }

        public bool ExistDir(string dir)
        {
            return m_baseFileSystemMock.ExistDir(dir);
        }

        public void EnumerateDirectory(string dir, Action<string> act)
        {
            dir = dir.ToLower();
            foreach (var sdir in m_files
                .Where(x => x.Key.StartsWith(dir))
                .Select(x=>Path.GetDirectoryName(x.Key).Substring(dir.Length))
                .Where(x=>x.Length>0)
                .Select(x=>x.TrimStart('\\'))
                .Where(x=>!x.Contains("\\") /* only direct children */)
                .Distinct())
            {
                act(sdir);
            }
        }

        public string GetContents(string file, bool failIfNotExists = false)
        {
            file = file.ToLower();
            if (!ExistFile(file))
            {
                if (failIfNotExists) throw new Exception();
                return null;
            }

            return m_files[file].Substring("CONTENT:".Length);
        }

        public KeyValuePair<string,string>? InternalReadLatest(string sourceDir)
        {
            sourceDir = sourceDir.ToLower();
            return m_files
                .Select(x => (KeyValuePair<string, string>?)x)
                .FirstOrDefault(x => x.HasValue && x.Value.Key.StartsWith(sourceDir));
        }

        public string PeekOldest(string sourceDir)
        {
            sourceDir = sourceDir.ToLower();
            var item = InternalReadLatest(sourceDir);

            if (!item.HasValue) return null;
            return item.Value.Value.Substring("CONTENT:".Length);
        }

        public bool MoveOldestToDir(
            string sourceDir,
            string workerDir,
            string successDir,
            string failureDir,
            Func<string, bool> processContents)
        {

            // Find oldest
            sourceDir = sourceDir.ToLower();
            var item = InternalReadLatest(sourceDir);

            if (item == null) return false;

            var fileName = item.Value.Key;
            var fileContents = item.Value.Value.Substring("CONTENT:".Length);


            // Move to worker directory during processing
            if (!Move(fileName, workerDir))
            {
                return false; // concurrent access happened
            }

            var newpath = Path.Combine(workerDir, Path.GetFileName(fileName));

            // Read contents and allow for processing, move appropriately

            if (processContents(fileContents))
            {
                if (!Move(newpath, successDir))
                {
                    throw new Exception(
                        "Could not move file from worker directory. Probably concurrent use of the same worker directory.");
                }
            }
            else
            {
                if (!Move(newpath, failureDir))
                {
                    throw new Exception(
                        "Could not move file from worker directory. Probably concurrent use of the same worker directory.");
                }
            }

            return true;
        }

        public void DeleteDirectoryIfEmpty(string dir)
        {
            // nothing to do because if files were there do not delete...
        }


        public bool ExistFile(string file)
        {
            return m_files.ContainsKey(file);
        }

        public void Remove(string file)
        {
            file = file.ToLower();
            m_files.Remove(file);
        }

        public bool Move(string file, string newdir)
        {
            if (!ExistFile(file)) return false;

            var contents = m_files[file].Substring("CONTENT:".Length);
            Remove(file);
            Save(Path.Combine(newdir, Path.GetFileName(file)), contents);
            return true;
        }

        public bool Save(string file, string contents, bool overwrite = false)
        {
            file = file.ToLower();
            if (!overwrite)
            {
                if (m_files.ContainsKey(file))
                {
                    return false;
                }
            }
            m_files.Add(file, "CONTENT:" + contents);
            return true;
        }
    }
}
