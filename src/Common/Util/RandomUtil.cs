using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Common.Util
{
    public static class RandomUtil
    {
        private static readonly Random rng = new Random();
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        private const string CHARS_COMPLEX = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!§$%&/()=?*+#-_";

        static RandomUtil()
        {
        }

        public static string String(int size = 20)
        {
            return String(size, CHARS);
        }

        public static string ComplexString(int size = 20)
        {
            return String(size, CHARS_COMPLEX);
        }

        private static string String(int size, string charset = CHARS_COMPLEX)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = charset[rng.Next(charset.Length)];
            }
            return new string(buffer);
        }

        public static int Int(int length, bool mustHaveLength = true)
        {
            var max = Convert.ToInt32(Math.Pow(10, length) - 1);
            var min = Convert.ToInt32(Math.Pow(10, length - 1));
            var rnd = rng.Next(max);
            if (!mustHaveLength)
            {
                return rnd;
            }
            else
            {
                while (rnd < min || rnd > max)
                {
                    while (rnd < min)
                    {
                        rnd = rnd + rng.Next(max);
                    }
                    while (rnd > max)
                    {
                        rnd = rnd - rng.Next(max);
                    }
                }
                return rnd;
            }
        }
    }
}
