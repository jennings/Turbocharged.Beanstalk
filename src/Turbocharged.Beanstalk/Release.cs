using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class ReleaseRequest : Request<bool>
    {
        public Task<bool> Task { get { return _tcs.Task; } }

        TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
        int _id;
        int _priority;
        int _delay;

        public ReleaseRequest(int id, int priority, int delay)
        {
            _id = id;
            _priority = priority;
            _delay = delay;
        }

        public byte[] ToByteArray()
        {
            return "release {0} {1} {2}\r\n".FormatWith(_id, _priority, _delay).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            switch (firstLine)
            {
                case "RELEASED":
                    _tcs.SetResult(true);
                    return;

                case "NOT_FOUND":
                case "BURIED":
                    // Should these both be false? Does a consumer need to distinguish?
                    // Should we return an enum instead?
                    _tcs.SetResult(false);
                    return;

                default:
                    Trace.Error("Unknown release response: {0}", firstLine);
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
