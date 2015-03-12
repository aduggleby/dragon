using System;
using System.Text;
using System.Security.Cryptography;
using Dragon.Context.Configuration;
using StructureMap;

namespace Dragon.Common.Util
{

    public class CryptUtil 
    {
        private const string CONFIG_KEY = "Dragon.Context.Encryption.Key";
        private const string CONFIG_IV = "Dragon.Context.Encryption.IV";

        private static IConfiguration m_configuration;

        static CryptUtil()
        {
            m_configuration = 
                ObjectFactory.Container.TryGetInstance<IConfiguration>();
        }

        public CryptUtil(IConfiguration configuration)
        {
            m_configuration = configuration;
        }

        public static string GenerateKey()
        {
            var crypto = new TripleDESCryptoServiceProvider();
            crypto.GenerateKey();
            return Convert.ToBase64String(crypto.Key);
        }

        public static string GenerateIV()
        {
            var crypto = new TripleDESCryptoServiceProvider();
            crypto.GenerateIV();
            return Convert.ToBase64String(crypto.IV);
        }

        public static string GetAndEnsureKeyFromWebConfig()
        {
            return m_configuration.EnsureValue<string>(CONFIG_KEY);
        }

        public static string GetAndEnsureIVFromWebConfig()
        {
            return m_configuration.EnsureValue<string>(CONFIG_IV);
        }

        public static string Encrypt(string unencrypted)
        {
            var key = Convert.FromBase64String(GetAndEnsureKeyFromWebConfig());
            var iv = Convert.FromBase64String(GetAndEnsureIVFromWebConfig());
            return Encrypt(key, iv, unencrypted);
        }

        public static string Decrypt(string encrypted)
        {
            var key = Convert.FromBase64String(GetAndEnsureKeyFromWebConfig());
            var iv = Convert.FromBase64String(GetAndEnsureIVFromWebConfig());
            return Decrypt(key, iv, encrypted);
        }

        public static string Encrypt(byte[] key, byte[] iv, string unencrypted)
        {
            var crypto = new TripleDESCryptoServiceProvider() { Key = key, IV = iv };

            var unencryptedBytes = Encoding.UTF8.GetBytes(unencrypted);

            return Convert.ToBase64String(
                crypto.CreateEncryptor().TransformFinalBlock(unencryptedBytes, 0, unencryptedBytes.Length));
        }

        public static string Decrypt(byte[] key, byte[] iv, string encrypted)
        {
            var crypto = new TripleDESCryptoServiceProvider() { Key = key, IV = iv };
            var encryptedBytes = Convert.FromBase64String(encrypted);

            return Encoding.UTF8.GetString(
                crypto.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
        }
    }
}

