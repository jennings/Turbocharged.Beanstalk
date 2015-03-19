using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    static class YamlHelper
    {
        public static Dictionary<string, string> ParseDictionary(byte[] buffer)
        {
            var result = new Dictionary<string, string>();
            using (var sr = new StreamReader(new MemoryStream(buffer), Encoding.ASCII))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length != 2) continue;
                    result.Add(parts[0].Trim(), parts[1].Trim());
                }
            }
            return result;
        }
    }
}
