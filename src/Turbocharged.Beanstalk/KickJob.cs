using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class KickJobRequest : Request<bool>
    {
        public Task<bool> Task { get { return _tcs.Task; } }
        int _id;

        TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        public KickJobRequest(int id)
        {
            _id = id;
        }

        public byte[] ToByteArray()
        {
            return "kick-job {0}\r\n".FormatWith(_id).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream, ILogger logger)
        {
            switch (firstLine)
            {
                case "KICKED": _tcs.SetResult(true); return;
                case "NOT_FOUND": _tcs.SetResult(false); return;
                default:
                    Reply.SetGeneralException(_tcs, firstLine, "kick-job", logger);
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
