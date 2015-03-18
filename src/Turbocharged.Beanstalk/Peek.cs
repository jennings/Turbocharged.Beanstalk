using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class PeekRequest : Request
    {
        public int Id { get; set; }
        public JobStatus Status { get; set; }

        TaskCompletionSource<JobDescription> _tcs;
        bool useStatus;

        public PeekRequest(JobStatus status, TaskCompletionSource<JobDescription> tcs)
        {
            useStatus = true;
            Status = status;
            _tcs = tcs;
        }

        public PeekRequest(int id, TaskCompletionSource<JobDescription> tcs)
        {
            Id = id;
            _tcs = tcs;
        }

        public byte[] ToByteArray()
        {
            if (useStatus)
            {
                switch (Status)
                {
                    case JobStatus.Ready:
                        return "peek-ready\r\n".ToASCIIByteArray();
                    case JobStatus.Delayed:
                        return "peek-delayed\r\n".ToASCIIByteArray();
                    case JobStatus.Buried:
                        return "peek-buried\r\n".ToASCIIByteArray();
                    default:
                        throw new NotSupportedException("Cannot peek jobs in status " + Status.ToString());
                }
            }
            else
                return "peek {0}\r\n".FormatWith(Id).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            switch (parts[0])
            {
                case "FOUND":
                    var id = Convert.ToInt32(parts[1]);
                    var bytes = Convert.ToInt32(parts[2]);
                    var descr = ReserveRequest.GetJobDescriptionFromBuffer(id, stream, bytes);
                    _tcs.SetResult(descr);
                    return;

                case "NOT_FOUND":
                    _tcs.SetResult(null);
                    return;

                default:
                    _tcs.SetException(new Exception("Unknown failure"));
                    return;
            }
        }
    }
}
