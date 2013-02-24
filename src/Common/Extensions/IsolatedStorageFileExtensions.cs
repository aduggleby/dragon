using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.IO;

namespace BootstrapCQRS.Extensions
{
    public static class IsolatedStorageFileExtensions
    {
        public static string GetFullPath(this IsolatedStorageFile isf)
        {
            return
                isf
                    .GetType()
                    .GetField("m_RootDir", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(isf).ToString();
        }
    }
}
