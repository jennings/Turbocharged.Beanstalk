using Microsoft.Extensions.Logging;
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
                        return PEEK_READY;
                    case JobState.Delayed:
                        return PEEK_DELAYED;
                    case JobState.Buried:
                        return PEEK_BURIED;
                    default:
                        throw new NotSupportedException("Cannot peek jobs in state " + _state.ToString());
                }
            }
            else
            {
                return "peek {0}\r\n".FormatWith(_id).ToASCIIByteArray();
            }
        }

        static readonly byte[] PEEK_READY = "peek-ready\r\n".ToASCIIByteArray();
        static readonly byte[] PEEK_DELAYED = "peek-delayed\r\n".ToASCIIByteArray();
        static readonly byte[] PEEK_BURIED = "peek-buried\r\n".ToASCIIByteArray();

        public void Process(string firstLine, NetworkStream stream, ILogger logger)
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
                    _tcs.SetResult(null);
                    return;

                default:
                    if (_id == null)
                    {
                        var command = _state == JobState.Ready ? "peek-ready" :
                                      _state == JobState.Buried ? "peek-buried" :
                                      _state == JobState.Delayed ? "peek-delayed" :
                                      "unknown peek";
                        Reply.SetGeneralException(_tcs, firstLine, command, logger);
                    }
                    else
                    {
                        Reply.SetGeneralException(_tcs, firstLine, "peek", logger);
                    }
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
