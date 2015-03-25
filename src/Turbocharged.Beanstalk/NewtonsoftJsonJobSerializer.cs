using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Turbocharged.Beanstalk
{
    class NewtonsoftJsonJobSerializer : IJobSerializer
    {
        public byte[] Serialize<T>(T job)
        {
            var str = JsonConvert.SerializeObject(job, Formatting.None);
            return Encoding.UTF8.GetBytes(str);
        }

        public T Deserialize<T>(byte[] buffer)
        {
            var str = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
