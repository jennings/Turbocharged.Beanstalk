using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated.Beanstalk
{
    class PutRequest : Request
    {
        const int MAX_JOB_SIZE = 1 << 16;

        public int Priority { get; set; }
        public int Delay { get; set; }
        public int TimeToRun { get; set; }
        public byte[] Job { get; set; }

        public PutRequest(TaskCompletionSource<int> tcs)
        {
            TimeToRun = 60;
            ResponseProcessor = new PutResponseProcessor(tcs);
        }

        public byte[] ToByteArray()
        {
            if (Job == null)
                throw new InvalidOperationException("Job must be set");
            if (Job.Length > MAX_JOB_SIZE)
                throw new InvalidOperationException("Maximum job size is 2^16 bytes");

            return "put {0} {1} {2} {3}\r\n".FormatWith(Priority, Delay, TimeToRun, Job.Length)
                .ToASCIIByteArray()
                .Concat(Job)
                .Concat(new byte[] { 13, 10 })
                .ToArray();
        }

        public ResponseProcessor ResponseProcessor { get; set; }

        class PutResponseProcessor : ResponseProcessor
        {
            TaskCompletionSource<int> _completionSource;

            public PutResponseProcessor(TaskCompletionSource<int> completionSource)
            {
                _completionSource = completionSource;
            }

            public void Process(string firstLine, NetworkStream stream)
            {
                var parts = firstLine.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0])
                {
                    case "INSERTED":
                        var id = Convert.ToInt32(parts[1]);
                        _completionSource.SetResult(id);
                        return;

                    case "BURIED":
                        var buriedId = Convert.ToInt32(parts[1]);
                        _completionSource.SetResult(buriedId);
                        return;

                    case "JOB_TOO_BIG":
                        _completionSource.SetException(new InvalidOperationException("Server is draining"));
                        return;

                    case "DRAINING":
                        _completionSource.SetException(new InvalidOperationException("Server is draining"));
                        return;

                    case "EXPECTED_CRLF":
                    default:
                        _completionSource.SetException(new InvalidOperationException("Unknown response: "+ parts[0]));
                        return;
                }
            }
        }
    }
}
