using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class WatchedRequest : Request
    {
        TaskCompletionSource<List<string>> _tcs;

        public WatchedRequest(TaskCompletionSource<List<string>> tcs)
        {
            _tcs = tcs;
        }

        public byte[] ToByteArray()
        {
            return "list-tubes-watched\r\n".ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            try
            {
                var parts = firstLine.Split(' ');
                if (parts.Length == 2 && parts[0] == "WATCHING")
                {
                    var bytes = Convert.ToInt32(parts[1]) + 2; // Get the last CRLF
                    var buffer = new byte[bytes];
                    stream.Read(buffer, 0, bytes);

                    var lines = buffer.ToASCIIString()
                        .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    _tcs.SetResult(lines);
                    return;
                }
            }
            catch
            {
            }

            _tcs.SetException(new Exception("Unknown use response"));
        }
    }
}
