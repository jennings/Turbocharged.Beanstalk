using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class PauseTubeRequest : Request<bool>
    {
        public Task<bool> Task { get { return _tcs.Task; } }
        Tube _tube;
        int _seconds;

        TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        public PauseTubeRequest(Tube tube, TimeSpan duration)
        {
            _tube = tube;
            _seconds = (int)duration.TotalSeconds;
        }

        public byte[] ToByteArray()
        {
            return "pause-tube {0} {1}\r\n".FormatWith(_tube.Name, _seconds).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            switch (firstLine)
            {
                case "PAUSED": _tcs.SetResult(true); return;
                case "NOT_FOUND": _tcs.SetResult(false); return;
                default:
                    Trace.Error("Unknown pause-tube response: {0}", firstLine);
                    _tcs.SetException(new Exception("Unknown response from pause-tube: '{0}'".FormatWith(firstLine)));
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
