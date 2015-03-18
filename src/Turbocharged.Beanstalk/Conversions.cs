using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    static class Conversions
    {
        public static char[] ToASCIICharArray(this string[] strings)
        {
            return string.Concat(strings).ToCharArray();
        }

        public static byte[] ToASCIIByteArray(this char[] chars)
        {
            return Encoding.ASCII.GetBytes(chars);
        }

        public static char[] ToASCIICharArray(this byte[] bytes)
        {
            return Encoding.ASCII.GetChars(bytes);
        }

        public static char[] ToASCIICharArray(this string unicodeString)
        {
            var unicodeChars = Encoding.Unicode.GetBytes(unicodeString);
            var asciiChars = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, unicodeChars);
            return Encoding.ASCII.GetChars(asciiChars);
        }

        public static byte[] ToASCIIByteArray(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
    }
}
