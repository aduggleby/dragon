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
    public class FastFileSystemEnumerator : IFileSystemEnumerator
    {
        public IEnumerable<FileData> EnumerateFiles(DirectoryInfo di)
        {
            return FastDirectoryEnumerator.EnumerateFiles(di.FullName);
        }
    }
}
