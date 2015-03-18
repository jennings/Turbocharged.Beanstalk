using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class UseRequest : Request
    {
        public string Tube { get; set; }

        TaskCompletionSource<string> _tcs;

        public UseRequest(string tube, TaskCompletionSource<string> tcs)
        {
            if (tube == null)
                throw new InvalidOperationException("Tube must not be null");

            Tube = tube;
            _tcs = tcs;
        }

        public byte[] ToByteArray()
        {
            return "use {0}\r\n".FormatWith(Tube)
                .ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(' ');
            if (parts.Length == 2 && parts[0] == "USING")
            {
                _tcs.SetResult(parts[1]);
            }
            else
            {
                _tcs.SetException(new Exception("Unknown use response"));
            }
        }
    }
}
