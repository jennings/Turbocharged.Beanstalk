using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class WatchRequest : Request<int>
    {
        public Task<int> Task { get { return _tcs.Task; } }
        public Tube Tube { get; set; }

        TaskCompletionSource<int> _tcs = new TaskCompletionSource<int>();

        public WatchRequest(Tube tube)
        {
            if (tube == null) throw new ArgumentNullException("tube");
            Tube = tube;
        }

        public byte[] ToByteArray()
        {
            return "watch {0}\r\n".FormatWith(Tube.Name)
                .ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream, ILogger logger)
        {
            var parts = firstLine.Split(' ');
            if (parts.Length == 2 && parts[0] == "WATCHING")
            {
                var num = Convert.ToInt32(parts[1]);
                _tcs.SetResult(num);
            }
            else
            {
                Reply.SetGeneralException(_tcs, firstLine, "watch", logger);
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
