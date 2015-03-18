using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated.Beanstalk
{
    class UseRequest : Request
    {
        public string Tube { get; set; }

        TaskCompletionSource<string> _tcs;

        public UseRequest(string tube, TaskCompletionSource<string> tcs)
        {
            Tube = tube;
            _tcs = tcs;
        }

        public byte[] ToByteArray()
        {
            if (Tube == null)
                throw new InvalidOperationException("Tube must not be null");

            return "use {0}\r\n".FormatWith(Tube)
                .ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(' ');
            if (parts.Length != 2)
            {
                _tcs.SetException(new Exception("Unknown use response"));
            }
            else
            {
                _tcs.SetResult(parts[1]);
            }
        }
    }
}
