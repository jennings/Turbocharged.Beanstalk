using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class PhysicalConnection
    {
        string _hostname;
        int _port;

        TcpClient _client;
        NetworkStream _stream;
        Task _receiveTask;

        BlockingCollection<Request> _requestsAwaitingResponse =
            new BlockingCollection<Request>(new ConcurrentQueue<Request>());

        public PhysicalConnection(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
            _client = new TcpClient();
        }

        public void Connect()
        {
            _client.Connect(_hostname, _port);
            _stream = _client.GetStream();
            _receiveTask = ReceiveAsync();
        }

        public void Close()
        {
            using (_stream)
                _client.Close();
        }

        public async Task SendAsync<T>(Request<T> request)
        {
            // TODO: Need a locking mechanism here so requests don't
            //       get inserted into the queue in a different order than
            //       they were sent over the wire (and therefore the
            //       order the server will respond in)

            _requestsAwaitingResponse.Add(request);
            var data = request.ToByteArray();
            await _stream.WriteAsync(data, 0, data.Length);
        }

        public async Task ReceiveAsync()
        {
            // So this is kind of dumb. But since I can't use a
            // StreamReader (can't get raw bytes out of it) or
            // a BinaryReader (no async methods), I'm reading one
            // character at a time so I don't read too much
            // (can't seek a NetworkStream).

            var max = 100;
            byte[] buffer = new byte[max];
            int pos = -1;
            while (true)
            {
                pos++;
                await _stream.ReadAsync(buffer, pos, 1);

                // We're done if the last two characters were CR LF
                if (pos > 0 && buffer[pos - 1] == 13 && buffer[pos] == 10)
                {
                    // We have a line, make it a string and send it to whoever is waiting for it.
                    var incoming = Encoding.ASCII.GetString(buffer, 0, pos - 1);
                    try
                    {
                        var request = _requestsAwaitingResponse.Take();
                        request.Process(incoming, _stream);
                        pos = -1;
                    }
                    catch (Exception ex)
                    {

                        Close();
                        throw;
                    }
                }
                else if (pos == max)
                {
                    throw new InvalidOperationException("Insufficient buffer for incoming message");
                }
            }
        }
    }
}
