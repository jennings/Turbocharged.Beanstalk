using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class DeleteRequest : Request<bool>
    {
        public Task<bool> Task { get { return _tcs.Task; } }

        TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
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
                    _tcs.SetResult(true);
                    return;

                case "NOT_FOUND":
                    // This isn't an exception because another consumer
                    // might have peeked and deleted the job while
                    // we were working on it. We don't want every consumer
                    // to always have to wrap the delete statement.
                    _tcs.SetResult(false);
                    return;

                default:
                    _tcs.SetException(new InvalidOperationException("Unknown response: " + firstLine));
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
