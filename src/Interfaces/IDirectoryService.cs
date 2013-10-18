using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Interfaces
{
    public interface IDirectoryService
    {
        bool Exists(string directory);
    }
}
