using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class PutRequest : Request<int>
    {
        const int MAX_JOB_SIZE = 1 << 16;

        public Task<int> Task { get { return _tcs.Task; } }
        int _priority;
        TimeSpan _delay;
        TimeSpan _timeToRun;
        byte[] _job;

        TaskCompletionSource<int> _tcs = new TaskCompletionSource<int>();

        public PutRequest(byte[] job, int priority, TimeSpan timeToRun, TimeSpan delay)
        {
            if (job == null)
                throw new ArgumentNullException("job");
            if (job.Length > MAX_JOB_SIZE)
                throw new ArgumentOutOfRangeException("job", "Maximum job size is " + MAX_JOB_SIZE + " bytes");

            _job = job;
            _priority = priority;
            _delay = delay;
            _timeToRun = timeToRun;
        }

        public byte[] ToByteArray()
        {
            var ttr = (int)_timeToRun.TotalSeconds;
            var delay = (int)_delay.TotalSeconds;
            return "put {0} {1} {2} {3}\r\n".FormatWith(_priority, delay, ttr, _job.Length)
                .ToASCIIByteArray()
                .Concat(_job)
                .Concat(new byte[] { 13, 10 })
                .ToArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            switch (parts[0])
            {
                case "INSERTED":
                    var id = Convert.ToInt32(parts[1]);
                    _tcs.SetResult(id);
                    return;

                case "BURIED":
                    var buriedId = Convert.ToInt32(parts[1]);
                    _tcs.SetResult(buriedId);
                    return;

                case "JOB_TOO_BIG":
                    _tcs.SetException(new InvalidOperationException("Server is draining"));
                    return;

                case "DRAINING":
                    _tcs.SetException(new InvalidOperationException("Server is draining"));
                    return;

                case "EXPECTED_CRLF":
                default:
                    _tcs.SetException(new InvalidOperationException("Unknown response: " + parts[0]));
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
