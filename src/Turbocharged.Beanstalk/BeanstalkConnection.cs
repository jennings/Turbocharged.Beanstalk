using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public sealed class BeanstalkConnection : IProducer, IConsumer, IDisposable
    {
        string _hostname;
        int _port;
        PhysicalConnection _connection;

        public BeanstalkConnection(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }

        public Task ConnectAsync()
        {
            _connection = new PhysicalConnection(_hostname, _port);
            return _connection.ConnectAsync();
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

        public IProducer GetProducer()
        {
            return this;
        }

        public IConsumer GetConsumer()
        {
            return this;
        }

        #region Producer

        Task<string> IProducer.Use(string tube)
        {
            var request = new UseRequest(tube);
            return SendAndGetResult(request);
        }

        Task<string> IProducer.Using()
        {
            var request = new UsingRequest();
            return SendAndGetResult(request);
        }

        Task<int> IProducer.PutAsync(byte[] job, int priority, int delay, int timeToRun)
        {
            var request = new PutRequest(job, priority, delay, timeToRun);
            return SendAndGetResult(request);
        }

        Task<Job> IProducer.PeekAsync()
        {
            return ((IProducer)this).PeekAsync(JobStatus.Ready);
        }

        Task<Job> IProducer.PeekAsync(JobStatus status)
        {
            var request = new PeekRequest(status);
            return SendAndGetResult(request);
        }

        Task<Job> IProducer.PeekAsync(int id)
        {
            var request = new PeekRequest(id);
            return SendAndGetResult(request);
        }

        Task<TubeStatistics> IProducer.TubeStatisticsAsync(string tube)
        {
            var request = new TubeStatisticsRequest(tube);
            return SendAndGetResult(request);
        }

        #endregion

        #region Consumer

        Task<int> IConsumer.Watch(string tube)
        {
            var request = new WatchRequest(tube);
            return SendAndGetResult(request);
        }

        Task<int> IConsumer.Ignore(string tube)
        {
            var request = new IgnoreRequest(tube);
            return SendAndGetResult(request);
        }

        Task<List<string>> IConsumer.Watched()
        {
            var request = new WatchedRequest();
            return SendAndGetResult(request);
        }

        Task<Job> IConsumer.ReserveAsync(TimeSpan timeout)
        {
            var request = new ReserveRequest(timeout);
            return SendAndGetResult(request);
        }

        Task<Job> IConsumer.ReserveAsync()
        {
            var request = new ReserveRequest();
            return SendAndGetResult(request);
        }

        Task<Job> IConsumer.PeekAsync(int id)
        {
            return ((IProducer)this).PeekAsync(id);
        }

        Task<bool> IConsumer.DeleteAsync(int id)
        {
            var request = new DeleteRequest(id);
            return SendAndGetResult(request);
        }

        Task<bool> IConsumer.ReleaseAsync(int id, int priority, TimeSpan delay)
        {
            var request = new ReleaseRequest(id, priority, (int)delay.TotalSeconds);
            return SendAndGetResult(request);
        }

        Task<bool> IConsumer.BuryAsync(int id, int priority)
        {
            var request = new BuryRequest(id, priority);
            return SendAndGetResult(request);
        }

        Task<bool> IConsumer.TouchAsync(int id)
        {
            var request = new TouchRequest(id);
            return SendAndGetResult(request);
        }

        Task<JobStatistics> IConsumer.JobStatisticsAsync(int id)
        {
            var request = new JobStatisticsRequest(id);
            return SendAndGetResult(request);
        }

        Task<TubeStatistics> IConsumer.TubeStatisticsAsync(string tube)
        {
            return ((IProducer)this).TubeStatisticsAsync(tube);
        }

        #endregion

        async Task<T> SendAndGetResult<T>(Request<T> request)
        {
            await _connection.SendAsync(request).ConfigureAwait(false);
            return await request.Task; // Let the consumer decide whether to ConfigureAwait or not
        }
    }
}
