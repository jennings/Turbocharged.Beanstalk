using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public class ConnectionConfiguration
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public IJobSerializer JobSerializer { get; set; }

        public ConnectionConfiguration()
        {
            JobSerializer = new NewtonsoftJsonJobSerializer();
        }

        public static ConnectionConfiguration Parse(string connectionString)
        {
            var parts = connectionString.Split(':');
            return new ConnectionConfiguration
            {
                Hostname = parts[0].Trim(),
                Port = Convert.ToInt32(parts[1].Trim()),
            };
        }

        public override string ToString()
        {
            return "{0}:{1}; JobSerializer={2}".FormatWith(
                Hostname,
                Port,
                JobSerializer == null ? "null" : JobSerializer.GetType().Name);
        }
    }
}
