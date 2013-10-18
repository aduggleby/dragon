using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Core.File
{
    public class DirectoryService : IDirectoryService
    {
        public bool Exists(string directory)
        {
            return Directory.Exists(directory);
        }
    }
}
