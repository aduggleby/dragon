using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dragon.Common.Util
{
    public class EmbeddedUtil
    {
        public static byte[] GetFileContents(string fileNamespace, string fileName)
        {
            return GetFileContents<EmbeddedUtil>(fileName, fileName);
        }

        public static byte[] GetFileContents(string fullPath)
        {
            return GetFileContents<EmbeddedUtil>(fullPath);
        }


        public static byte[] GetFileContents<T>(string fileNamespace, string fileName)
        {
            var fullPath = fileNamespace.TrimEnd('.') + "." + fileName;

            return GetFileContents(fullPath);
        }

        public static byte[] GetFileContents<T>(string fullPath)
        {
            var f = ActualFileCasing(fullPath, assembly: typeof(T).Assembly);

            if (f == null)
            {
                throw new Exception(string.Format("File not found: '{0}'", fullPath));
            }

            using (Stream stream = typeof(T).Assembly.GetManifestResourceStream(f))
            {
                return stream.ReadAllBytes();
            }
        }

        public static bool ExistFile(string fileNamespace, string fileName)
        {
            var fullPath = fileNamespace.TrimEnd('.') + "." + fileName;

            return ExistFile(fullPath);
        }

        public static bool ExistFile(string fullPath)
        {
            return (ActualFileCasing(fullPath) != null);
        }

        public static string ActualFileCasing(string fullPath, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = System.Reflection.Assembly.GetCallingAssembly();
            }
            var s = assembly.GetManifestResourceNames();

            foreach (var i in s)
            {
                if (i.Equals(fullPath, StringComparison.InvariantCultureIgnoreCase)) return i;
            }
            return null;
        }


    }

    public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }
        }
    }

}
