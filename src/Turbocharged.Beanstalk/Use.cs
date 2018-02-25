using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class UseRequest : Request<string>
    {
        public Task<string> Task { get { return _tcs.Task; } }
        public Tube Tube { get; set; }

        TaskCompletionSource<string> _tcs = new TaskCompletionSource<string>();

        public UseRequest(Tube tube)
        {
            if (tube == null) throw new ArgumentNullException("tube");
            Tube = tube;
        }

        public byte[] ToByteArray()
        {
            return "use {0}\r\n".FormatWith(Tube.Name)
                .ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream, ILogger logger)
        {
            var parts = firstLine.Split(' ');
            if (parts.Length == 2 && parts[0] == "USING")
            {
                _tcs.SetResult(parts[1]);
            }
            else
            {
                Reply.SetGeneralException(_tcs, firstLine, "use", logger);
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
