using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Context.Util
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
            if (length <= 0) throw new ArgumentException("Cannot create integers with length 0 or less", "length");

            // 2147483647 = max, only allow lenght <= 10
            if (length >= 11) throw new ArgumentException("Cannot create integers with length 10", "length");

            var max = Convert.ToInt32(Math.Min(Int32.MaxValue, Math.Pow(10, length) - 1));
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
