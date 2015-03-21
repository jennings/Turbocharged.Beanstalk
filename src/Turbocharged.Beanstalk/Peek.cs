using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class PeekRequest : Request<Job>
    {
        public Task<Job> Task { get { return _tcs.Task; } }
        int? _id;
        JobState _state { get; set; }

        TaskCompletionSource<Job> _tcs = new TaskCompletionSource<Job>();

        public PeekRequest(JobState state)
        {
            if (state != JobState.Ready
                && state != JobState.Delayed
                && state != JobState.Buried)
                throw new ArgumentOutOfRangeException("Cannot peek jobs in state " + state.ToString());

            _state = state;
            _id = null;
        }

        public PeekRequest(int id)
        {
            _id = id;
        }

        public byte[] ToByteArray()
        {
            if (_id == null)
            {
                switch (_state)
                {
                    case JobState.Ready:
                        return "peek-ready\r\n".ToASCIIByteArray();
                    case JobState.Delayed:
                        return "peek-delayed\r\n".ToASCIIByteArray();
                    case JobState.Buried:
                        return "peek-buried\r\n".ToASCIIByteArray();
                    default:
                        throw new NotSupportedException("Cannot peek jobs in state " + _state.ToString());
                }
            }
            else
            {
                return "peek {0}\r\n".FormatWith(_id).ToASCIIByteArray();
            }
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            switch (parts[0])
            {
                case "FOUND":
                    var id = Convert.ToInt32(parts[1]);
                    var bytes = Convert.ToInt32(parts[2]);
                    Job descr;
                    if (ReserveRequest.TryGetJobFromBuffer(id, stream, bytes, out descr))
                        _tcs.SetResult(descr);
                    else
                        _tcs.SetException(new Exception("Unable to parse job description"));
                    return;

                case "NOT_FOUND":
                    // If we searched by ID, should we instead throw KeyNotFoundException?
                    _tcs.SetResult(null);
                    return;

                default:
                    _tcs.SetException(new Exception("Unknown failure"));
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
