using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    static class EncodingExtensions
    {
        public static byte[] ToASCIIByteArray(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string ToASCIIString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static string ToASCIIString(this byte[] bytes, int index, int count)
        {
            return Encoding.ASCII.GetString(bytes, index, count);
        }
    }
}
