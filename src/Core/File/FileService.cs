using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Core.File
{
    public class FileService : IFileService
    {
        public string[] GetFileContents(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        public bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }
    }
}
