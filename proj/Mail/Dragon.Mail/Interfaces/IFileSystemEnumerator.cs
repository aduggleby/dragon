using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Utils;

namespace Dragon.Mail.Interfaces
{
    public interface IFileSystemEnumerator
    {
        IEnumerable<FileData> EnumerateFiles(DirectoryInfo di);
    }
}
