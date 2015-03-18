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

        public UseRequest(string tube, TaskCompletionSource<string> tcs)
        {
            ResponseProcessor = new UseResponseProcessor(tcs);
        }

        public byte[] ToByteArray()
        {
            if (Tube == null)
                throw new InvalidOperationException("Tube must not be null");

            return "use {0}\r\n".FormatWith(Tube)
                .ToASCIIByteArray();
        }

        public ResponseProcessor ResponseProcessor { get; set; }

        class UseResponseProcessor : ResponseProcessor
        {
            TaskCompletionSource<string> _completionSource;

            public UseResponseProcessor(TaskCompletionSource<string> completionSource)
            {
                _completionSource = completionSource;
            }

            public void Process(string firstLine, NetworkStream stream)
            {
                var parts = firstLine.Split(' ');
                if (parts.Length != 2)
                {
                    _completionSource.SetException(new Exception("Unknown use response"));
                }
                else
                {
                    _completionSource.SetResult(parts[1]);
                }
            }
        }
    }
}
