using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class WatchRequest : Request<int>
    {
        public Task<int> Task { get { return _tcs.Task; } }
        public string Tube { get; set; }

        TaskCompletionSource<int> _tcs = new TaskCompletionSource<int>();

        public WatchRequest(string tube)
        {
            if (tube == null)
                throw new InvalidOperationException("Tube must not be null");

            Tube = tube;
        }

        public byte[] ToByteArray()
        {
            return "watch {0}\r\n".FormatWith(Tube)
                .ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(' ');
            if (parts.Length == 2 && parts[0] == "WATCHING")
            {
                var num = Convert.ToInt32(parts[1]);
                _tcs.SetResult(num);
            }
            else
            {
                _tcs.SetException(new Exception("Unknown watch response"));
            }
        }
    }
}
