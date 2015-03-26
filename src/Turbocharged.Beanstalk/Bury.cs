using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class BuryRequest : Request<bool>
    {
        public Task<bool> Task { get { return _tcs.Task; } }

        TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
        int _id;
        int _priority;

        public BuryRequest(int id, int priority)
        {
            _id = id;
            _priority = priority;
        }

        public byte[] ToByteArray()
        {
            return "bury {0} {1}\r\n".FormatWith(_id, _priority).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            switch (firstLine)
            {
                case "BURIED":
                    _tcs.SetResult(true);
                    return;

                case "NOT_FOUND":
                    _tcs.SetResult(false);
                    return;

                default:
                    Trace.Error("Unknown bury response: {0}", firstLine);
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
