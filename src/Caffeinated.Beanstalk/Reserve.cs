using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated.Beanstalk
{
    class ReserveRequest : Request
    {
        public ReserveRequest(TaskCompletionSource<JobDescription> tcs)
        {
            ResponseProcessor = new ReserveResponseProcessor(tcs);
        }

        public byte[] ToByteArray()
        {
            return "reserve\r\n".ToASCIIByteArray();
        }

        public ResponseProcessor ResponseProcessor { get; set; }

        class ReserveResponseProcessor : ResponseProcessor
        {
            TaskCompletionSource<JobDescription> _completionSource;

            public ReserveResponseProcessor(TaskCompletionSource<JobDescription> completionSource)
            {
                _completionSource = completionSource;
            }

            public void Process(string firstLine, NetworkStream stream)
            {
                var parts = firstLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0])
                {
                    case "RESERVED":
                        var id = Convert.ToInt32(parts[1]);
                        var bytes = Convert.ToInt32(parts[2]);
                        var buffer = new byte[bytes];
                        var readBytes = stream.Read(buffer, 0, bytes);
                        if (readBytes != bytes)
                        {
                            // TODO: Now what, genius?
                        }
                        stream.ReadByte(); // CR
                        stream.ReadByte(); // LF

                        var descr = new JobDescription
                        {
                            Id = id,
                            JobData = buffer,
                        };
                        _completionSource.SetResult(descr);
                        return;

                    case "DEADLINE_SOON":
                        // TODO: WTF do I do with this? Reschedule?
                        _completionSource.SetException(new Exception("Deadline soon"));
                        return;

                    default:
                        _completionSource.SetException(new Exception("Unknown failure"));
                        return;
                }
            }
        }
    }
}
