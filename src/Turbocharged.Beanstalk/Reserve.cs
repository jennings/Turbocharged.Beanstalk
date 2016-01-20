using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class ReserveRequest : Request<Job>
    {
        public Task<Job> Task { get { return _tcs.Task; } }

        TaskCompletionSource<Job> _tcs = new TaskCompletionSource<Job>();
        TimeSpan? _timeout;

        /// <summary>
        /// Creates a "reserve" request which will last until
        /// a job is reserved, the timeout expires, or the
        /// connection is torn down.
        /// </summary>
        public ReserveRequest(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        /// <summary>
        /// Creates a "reserve" request which will last until
        /// a job is reserved or the connection is torn down.
        /// </summary>
        public ReserveRequest()
        {
            _timeout = null;
        }

        public byte[] ToByteArray()
        {
            if (_timeout.HasValue)
                return "reserve-with-timeout {0}\r\n".FormatWith((int)_timeout.Value.TotalSeconds).ToASCIIByteArray();
            else
                return "reserve\r\n".ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            switch (parts[0])
            {
                case "RESERVED":
                    var id = Convert.ToInt32(parts[1]);
                    var bytes = Convert.ToInt32(parts[2]);
                    Job descr;
                    if (TryGetJobFromBuffer(id, stream, bytes, out descr))
                        _tcs.SetResult(descr);
                    else
                        _tcs.SetException(new Exception("Unable to parse job description"));
                    return;

                case "TIMED_OUT":
                    _tcs.SetException(new TimeoutException("Timeout expired waiting to reserve a job"));
                    return;

                case "DEADLINE_SOON":
                    _tcs.SetResult(null);
                    return;

                default:
                    var command = _timeout.HasValue ? "reserve-with-timeout" : "reserve";
                    Reply.SetGeneralException(_tcs, firstLine, command);
                    return;
            }
        }

        internal static bool TryGetJobFromBuffer(int id, NetworkStream stream, int bytes, out Job job)
        {
            var buffer = new byte[bytes];
            var readBytes = stream.Read(buffer, 0, bytes);
            if (readBytes != bytes)
            {
                job = null;
                return false;
            }
            stream.ReadByte(); // CR
            stream.ReadByte(); // LF

            job = new Job
            {
                Id = id,
                Data = buffer,
            };
            return true;
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
