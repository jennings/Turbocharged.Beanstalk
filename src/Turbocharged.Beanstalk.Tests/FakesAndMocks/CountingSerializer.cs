using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Turbocharged.Beanstalk.Tests.FakesAndMocks
{
    class CountingSerializer : IJobSerializer
    {
        public int SerializeCount = 0;
        public int DeserializeCount = 0;

        public byte[] Serialize<T>(T job)
        {
            SerializeCount++;
            var str = JsonConvert.SerializeObject(job, Formatting.None);
            return Encoding.UTF8.GetBytes(str);
        }

        public T Deserialize<T>(byte[] buffer)
        {
            DeserializeCount++;
            var str = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
