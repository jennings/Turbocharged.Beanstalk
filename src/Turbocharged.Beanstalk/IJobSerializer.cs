using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    /// <summary>
    /// Defines a particular strategy for serializing objects into byte arrays for insertion into Beanstalk.
    /// </summary>
    public interface IJobSerializer
    {
        /// <summary>
        /// Serializes a job to a byte array suitable for insertion into Beanstalk.
        /// </summary>
        byte[] Serialize<T>(T job);

        /// <summary>
        /// Deserializes a byte array back into a typed object.
        /// </summary>
        T Deserialize<T>(byte[] buffer);
    }

    class NotProvidedSerializer : IJobSerializer
    {
        public byte[] Serialize<T>(T job)
        {
            throw new InvalidOperationException("An IJobSerializer must be provided when establishing a BeanstalkConnection");
        }

        public T Deserialize<T>(byte[] buffer)
        {
            throw new InvalidOperationException("An IJobSerializer must be provided when establishing a BeanstalkConnection");
        }
    }
}
