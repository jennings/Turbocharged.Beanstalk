using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class UsingRequest : Request<string>
    {
        public Task<string> Task { get { return _tcs.Task; } }

        TaskCompletionSource<string> _tcs = new TaskCompletionSource<string>();

        public byte[] ToByteArray()
        {
            return "list-tube-used\r\n".ToASCIIByteArray();
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

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
