using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System.IO
{
    public static class DirectoryInfoExtensions
    {
        [DebuggerNonUserCode]
        public static string GetRelativeName(this DirectoryInfo di, DirectoryInfo baseDir)
        {
            if (di.FullName.ToLower().Contains(baseDir.FullName.ToLower()))
            {
                return di.FullName.Substring(baseDir.FullName.Length + 1);
            }
            else
            {
                return di.FullName;
            }
        }

        [DebuggerNonUserCode]
        public static FileInfo GetFile(this  DirectoryInfo di, string file)
        {
            return di.EnumerateFiles().FirstOrDefault(x => x.Name == file);
        }

        [DebuggerNonUserCode]
        public static FileInfo CopyFile(this  DirectoryInfo di, string file, DirectoryInfo toDir)
        {
            var f = di.EnumerateFiles().FirstOrDefault(x => x.Name == file);

            return f.CopyTo(Path.Combine(toDir.FullName, file));
        }

        [DebuggerNonUserCode]
        public static DirectoryInfo CopyFileWithOnTheFlyReplacement(this  DirectoryInfo di, DirectoryInfo toDir, Func<string, string> replacement, string file)
        {
            var newfile = di.CopyFile(file, toDir);
            newfile.ParseAndReplaceContent(replacement);
            return di;
        }

        [DebuggerNonUserCode]
        public static DirectoryInfo CopyFilesWithOnTheFlyReplacement(this  DirectoryInfo di, DirectoryInfo toDir, Func<string, string> replacement, params string[] files)
        {
            foreach (var file in files) di.CopyFileWithOnTheFlyReplacement(toDir, replacement, file);
            return di;
        }

        [DebuggerNonUserCode]
        public static DirectoryInfo GetDirectory(this DirectoryInfo di, string dir)
        {
            var parts = dir.Split(Path.DirectorySeparatorChar);

            var theDir = di;
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;
                theDir = theDir.EnumerateDirectories().FirstOrDefault(x => x.Name == part);

                if (theDir == null) throw new Exception("Path element '" + part + "' not found in '" + dir + "'.");
            }

            return theDir;
        }
    }
}
