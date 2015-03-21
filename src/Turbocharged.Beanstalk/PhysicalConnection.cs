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
    class PhysicalConnection : IDisposable
    {
        string _hostname;
        int _port;

        TcpClient _client;
        NetworkStream _stream;
        IDisposable _receiveTask;

        BlockingCollection<Request> _requestsAwaitingResponse =
            new BlockingCollection<Request>(new ConcurrentQueue<Request>());

        public PhysicalConnection(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
            _client = new TcpClient();
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(_hostname, _port);
            _stream = _client.GetStream();
            var cts = new CancellationTokenSource();

#pragma warning disable 4014
            ReceiveAsync(cts.Token);
#pragma warning restore 4014

            _receiveTask = Disposable.Create(() =>
            {
                cts.Cancel();
                cts.Dispose();
            });
        }

        public void Dispose()
        {
            using (_client)
            using (_stream)
            {
                _receiveTask.Dispose();
                _client.Close();
            }
        }

        public Task SendAsync<T>(Request<T> request, CancellationToken cancellationToken)
        {
            // TODO: Need a locking mechanism here so requests don't
            //       get inserted into the queue in a different order than
            //       they were sent over the wire (and therefore the
            //       order the server will respond in)

            _requestsAwaitingResponse.Add(request);
            var data = request.ToByteArray();
            return _stream.WriteAsync(data, 0, data.Length, cancellationToken);
        }

        public async Task ReceiveAsync(CancellationToken token)
        {
            try
            {
                var firstLineMaxLength = 255;
                byte[] buffer = new byte[firstLineMaxLength];
                int pos = -1; // Our current position in the buffer
                while (true)
                {
                    pos++;
                    try
                    {
                        // So this is kind of dumb. But since I can't use a
                        // StreamReader (can't get raw bytes out of it)
                        // or a BinaryReader (no async methods) and I don't
                        // want to deal with buffering myself, I'm reading
                        // one character at a time so I don't read past the CRLF
                        await _stream.ReadAsync(buffer, pos, 1, token);
                    }
                    catch (OperationCanceledException)
                    {
                        Drain();
                        return;
                    }

                    // We're done if the last two characters were CR LF
                    if (pos > 0 && buffer[pos - 1] == 13 && buffer[pos] == 10)
                    {
                        // We have a line, make it a string and send it to whoever is waiting for it.
                        var incoming = buffer.ToASCIIString(0, pos - 1);
                        var request = _requestsAwaitingResponse.Take(token);
                        try
                        {
                            request.Process(incoming, _stream);
                        }
                        catch (Exception)
                        {
                            // How rude
                        }
                        pos = -1; // Overwrite the buffer on the next go-round
                    }
                    else if (pos == firstLineMaxLength)
                    {
                        // Oops
                        throw new InvalidOperationException("Unable to read message from server");
                    }
                }
            }
            catch (Exception)
            {
                // No way to really recover from this
                Dispose();
            }
        }

        void Drain()
        {
            Request request;
            while (_requestsAwaitingResponse.TryTake(out request))
            {
                request.Cancel();
            }
        }
    }
}
