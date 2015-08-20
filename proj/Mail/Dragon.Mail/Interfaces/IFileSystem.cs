using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Interfaces
{
    public interface IFileSystem
    {
        bool CreateDir(string dir, bool ignoreIfExists = true);
        bool ExistDir(string dir);

        void EnumerateDirectory(string dir, Action<string> act);
        string GetContents(string file, bool failIfNotExists = false);

        string PeekOldest(string dir);

        bool MoveOldestToDir(
            string sourceDir,
            string workerDir,
            string successDir,
            string failureDir,
            Func<string, bool> processContents);

        void DeleteDirectoryIfEmpty(string dir);

        bool ExistFile(string file);
        void Remove(string file);
        bool Move(string file, string newdir);
        bool Save(string file, string contents, bool overwrite = false);

    }
}
