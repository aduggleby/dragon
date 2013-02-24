using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class FileInfoExtensions
    {
        public static string GetRelativeName(this FileInfo fi, DirectoryInfo baseDir)
        {
            if (fi.FullName.ToLower().Contains(baseDir.FullName.ToLower()))
            {
                return fi.FullName.Substring(baseDir.FullName.Length + 1);
            }
            else
            {
                return fi.FullName;
            }
        }

        public static FileInfo ParseAndReplaceContent(this FileInfo fi, Func<string, string> replacement)
        {
            File.WriteAllText(fi.FullName, replacement(File.ReadAllText(fi.FullName)));
            return fi;
        }
    }
}
