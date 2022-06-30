using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public class ConnectionConfiguration
    {
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 11300;
        public IJobSerializer JobSerializer { get; set; }
        public ILogger Logger { get; set; }

        public ConnectionConfiguration()
        {
            JobSerializer = new SystemTextJsonJobSerializer();
        }

        public static ConnectionConfiguration Parse(string connectionString)
        {
            var parts = connectionString.Split(new []{ ';' }, StringSplitOptions.RemoveEmptyEntries);
            var config = new ConnectionConfiguration();
            foreach (var part in parts)
            {
                var kvp = part.Split(new []{ '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (kvp.Length == 1)
                {
                    var hostParts = kvp[0].Split(':');
                    config.Hostname = hostParts[0].Trim();
                    if (hostParts.Length == 2)
                    {
                        config.Port = int.Parse(hostParts[1]);
                    }
                }
                else
                {
                    // No other options yet
                }
            }

            return config;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Hostname);
            if (Port != 11300)
                sb.Append(":" + Port);
            if (JobSerializer != null)
                sb.Append("; JobSerializer=" + JobSerializer.ToString());
            if (Logger != null)
                sb.Append("; Logger=" + Logger.ToString());
            return sb.ToString();
        }
    }
}
