using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class DeleteRequest : Request<int>
    {
        public Task<int> Task { get { return _tcs.Task; } }

        TaskCompletionSource<int> _tcs = new TaskCompletionSource<int>();
        int _id;

        public DeleteRequest(int id)
        {
            _id = id;
        }

        public byte[] ToByteArray()
        {
            return "delete {0}\r\n".FormatWith(_id).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            switch (firstLine)
            {
                case "DELETED":
                    _tcs.SetResult(0);
                    return;

                case "NOT_FOUND":
                    _tcs.SetException(new KeyNotFoundException("Job ID not found"));
                    return;

                default:
                    _tcs.SetException(new InvalidOperationException("Unknown response: " + firstLine));
                    return;
            }
        }
    }
}
