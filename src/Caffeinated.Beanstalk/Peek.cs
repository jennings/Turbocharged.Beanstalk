using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated.Beanstalk
{
    class PeekRequest : Request
    {
        public int Id { get; set; }

        public PeekRequest(int id, TaskCompletionSource<JobDescription> tcs)
        {
            Id = id;
            ResponseProcessor = new PeekResponseProcessor(tcs);
        }

        public byte[] ToByteArray()
        {
            return "peek {0}\r\n".FormatWith(Id).ToASCIIByteArray();
        }

        public ResponseProcessor ResponseProcessor { get; set; }

        class PeekResponseProcessor : ResponseProcessor
        {
            TaskCompletionSource<JobDescription> _completionSource;

            public PeekResponseProcessor(TaskCompletionSource<JobDescription> completionSource)
            {
                _completionSource = completionSource;
            }

            public void Process(string firstLine, NetworkStream stream)
            {
                var parts = firstLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0])
                {
                    case "FOUND":
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

                    case "NOT_FOUND":
                        _completionSource.SetException(new InvalidOperationException("Deadline soon"));
                        return;

                    default:
                        _completionSource.SetException(new Exception("Unknown failure"));
                        return;
                }
            }
        }
    }
}
