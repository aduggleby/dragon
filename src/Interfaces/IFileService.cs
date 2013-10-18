using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces
{
    public interface IFileService
    {
        string[] GetFileContents(string path);
        bool Exists(string path);
    }
}
