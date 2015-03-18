using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated.Beanstalk
{
    class IgnoreRequest : Request
    {
        public string Tube { get; set; }

        TaskCompletionSource<int> _tcs;

        public IgnoreRequest(string tube, TaskCompletionSource<int> tcs)
        {
            if (tube == null)
                throw new InvalidOperationException("Tube must not be null");

            Tube = tube;
            _tcs = tcs;
        }

        public byte[] ToByteArray()
        {
            return "ignore {0}\r\n".FormatWith(Tube)
                .ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(' ');
            if (parts.Length == 0)
                throw new Exception("Empty ignore response");

            switch (parts[0])
            {
                case "WATCHING":
                    if (parts.Length == 2)
                    {
                        var num = Convert.ToInt32(parts[1]);
                        _tcs.SetResult(num);
                    }
                    else
                    {
                        _tcs.SetException(new Exception("Unknown ignore response"));
                    }
                    return;

                case "NOT_IGNORED":
                    _tcs.SetException(new InvalidOperationException("Must be watching at least one tube"));
                    return;
            }
        }
    }
}
