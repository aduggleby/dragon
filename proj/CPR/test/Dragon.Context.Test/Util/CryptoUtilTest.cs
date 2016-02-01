using System;
using System.Diagnostics;
using System.Linq;
using Dragon.Common.Util;
using Dragon.Context.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Context.Test.Util
{
    [TestClass]
    public class CryptoUtilTest
    {
        [TestMethod]
        public void TestEncrypt_validArguments_shouldEncrypt()
        {
            InitCryptUtil();
            var text = RandomString(99999999);

            // TODO: loop
            var begin = Process.GetCurrentProcess().TotalProcessorTime;
            CryptUtil.Encrypt(text);
            var end = Process.GetCurrentProcess().TotalProcessorTime;
            Debug.WriteLine("Processor time: " + (end - begin).TotalMilliseconds + " ms.");

            var begin2 = DateTime.UtcNow;
            CryptUtil.Encrypt(text);
            var end2 = DateTime.UtcNow;
            Trace.WriteLine("Date time: " + (end2-begin2).TotalMilliseconds + " ms.");
        }

        #region helpers

        private static void InitCryptUtil()
        {
            var configuration = new InMemoryConfiguration();
            configuration.Set("Dragon.Context.Encryption.Key", CryptUtil.GenerateKey());
            configuration.Set("Dragon.Context.Encryption.IV", CryptUtil.GenerateIV());
            // ReSharper disable once ObjectCreationAsStatement
            new CryptUtil(configuration);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion
    }
}
