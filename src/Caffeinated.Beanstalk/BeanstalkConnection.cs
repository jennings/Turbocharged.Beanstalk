using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Caffeinated.Beanstalk
{
    public sealed class BeanstalkConnection : IDisposable
    {
        string _hostname;
        int _port;
        PhysicalConnection _connection;

        public BeanstalkConnection(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }

        public void Connect()
        {
            _connection = new PhysicalConnection(_hostname, _port);
            _connection.Connect();
        }

        public void Close()
        {
            try
            {
                _connection.Close();
            }
            finally
            {
                _connection = null;
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public async Task<string> Use(string tube)
        {
            var tcs = new TaskCompletionSource<string>();
            var request = new UseRequest(tube, tcs);
            await _connection.SendAsync(request);
            return await tcs.Task.ConfigureAwait(false);
        }

        public async Task<int> PutAsync(byte[] job, int priority, int delay, int ttr)
        {
            var tcs = new TaskCompletionSource<int>();
            var request = new PutRequest(tcs)
            {
                Priority = priority,
                Delay = delay,
                TimeToRun = ttr,
                Job = job,
            };
            await _connection.SendAsync(request);
            return await tcs.Task.ConfigureAwait(false);
        }

        public async Task<JobDescription> PeekAsync(JobStatus status)
        {
            var tcs = new TaskCompletionSource<JobDescription>();
            var request = new PeekRequest(status, tcs);
            await _connection.SendAsync(request).ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }

        public async Task<JobDescription> PeekAsync(int id)
        {
            var tcs = new TaskCompletionSource<JobDescription>();
            var request = new PeekRequest(id, tcs);
            await _connection.SendAsync(request).ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }

        public async Task<JobDescription> ReserveAsync(TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<JobDescription>();
            var request = new ReserveRequest(timeout, tcs);
            await _connection.SendAsync(request).ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }

        public async Task<JobDescription> ReserveAsync()
        {
            var tcs = new TaskCompletionSource<JobDescription>();
            var request = new ReserveRequest(tcs);
            await _connection.SendAsync(request).ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }
    }
}
