using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Turbocharged.Beanstalk.Tests.FakesAndMocks
{
    class ThrowingSerializer : IJobSerializer
    {
        public IReadOnlyList<int> ReceivedJobIds { get { return _receivedIds.AsReadOnly(); } }

        List<int> _receivedIds = new List<int>();

        public byte[] Serialize<T>(T job)
        {
            throw new Exception("Unable to serialize job.");
        }

        public T Deserialize<T>(byte[] buffer)
        {
            throw new Exception("Unable to deserialize job.");
        }
    }
}
