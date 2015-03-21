using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    using WorkerFunc = Func<IWorker, Job, Task>;

    /// <summary>
    /// A connection to a Beanstalk server.
    /// </summary>
    public sealed class BeanstalkConnection : IProducer, IConsumer, IDisposable
    {
        string _hostname;
        int _port;
        PhysicalConnection _connection;

        #region Connection

        // Private, so we can make the object creation async
        BeanstalkConnection(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
            _connection = new PhysicalConnection(hostname, port);
        }

        static async Task<BeanstalkConnection> ConnectAsync(string hostname, int port)
        {
            var connection = new BeanstalkConnection(hostname, port);
            await connection._connection.ConnectAsync(); // Yo dawg
            return connection;
        }

        /// <summary>
        /// Creates a consumer with a dedicated TCP connection to a Beanstalk server.
        /// </summary>
        public static async Task<IConsumer> ConnectConsumerAsync(string hostname, int port)
        {
            return await ConnectAsync(hostname, port);
        }

        /// <summary>
        /// Creates a producer with a dedicated TCP connection to a Beanstalk server.
        /// </summary>
        public static async Task<IProducer> ConnectProducerAsync(string hostname, int port)
        {
            return await ConnectAsync(hostname, port);
        }

        /// <summary>
        /// Schedulers a worker with a dedicated TCP connection to repeatedly reserve jobs and process them.
        /// </summary>
        public static async Task<IDisposable> ConnectWorkerAsync(string hostname, int port, WorkerFunc worker)
        {
            var conn = await BeanstalkConnection.ConnectAsync(hostname, port);
            var cts = new CancellationTokenSource();
            var task = conn.WorkerLoop(worker, cts.Token);
            return Disposable.Create(() =>
            {
                cts.Cancel();
                cts.Dispose();
                conn.Dispose();
            });
        }

        Task WorkerLoop(WorkerFunc worker, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var job = await ReserveAsync(cancellationToken).ConfigureAwait(false);
                    await worker(this, job).ConfigureAwait(false);
                }
            }, cancellationToken);
        }

        public void Dispose()
        {
            var c = Interlocked.Exchange(ref _connection, null);
            if (c != null)
            {
                c.Dispose();
            }
        }

        #endregion

        #region Producer

        Task<string> IProducer.UseAsync(string tube)
        {
            var request = new UseRequest(tube);
            return SendAndGetResult(request);
        }

        Task<string> IProducer.UsingAsync()
        {
            var request = new UsingRequest();
            return SendAndGetResult(request);
        }

        Task<int> IProducer.PutAsync(byte[] job, int priority, TimeSpan delay, TimeSpan timeToRun)
        {
            var request = new PutRequest(job, priority, delay, timeToRun);
            return SendAndGetResult(request);
        }

        Task<Job> IProducer.PeekAsync()
        {
            return ((IProducer)this).PeekAsync(JobState.Ready);
        }

        Task<Job> IProducer.PeekAsync(JobState state)
        {
            var request = new PeekRequest(state);
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

        #endregion

        #region Worker

        // This is purposefully private and not part of the IConsumer
        // interface, it exists for worker loops
        Task<Job> ReserveAsync(CancellationToken cancellationToken)
        {
            var request = new ReserveRequest();
            return SendAndGetResult(request, cancellationToken);
        }

        Task<int> IWorker.WatchAsync(string tube)
        {
            var request = new WatchRequest(tube);
            return SendAndGetResult(request);
        }

        Task<int> IWorker.IgnoreAsync(string tube)
        {
            var request = new IgnoreRequest(tube);
            return SendAndGetResult(request);
        }

        Task<List<string>> IWorker.WatchedAsync()
        {
            var request = new WatchedRequest();
            return SendAndGetResult(request);
        }

        Task<Job> IWorker.PeekAsync(int id)
        {
            return ((IProducer)this).PeekAsync(id);
        }

        Task<bool> IWorker.DeleteAsync(int id)
        {
            var request = new DeleteRequest(id);
            return SendAndGetResult(request);
        }

        Task<bool> IWorker.ReleaseAsync(int id, int priority, TimeSpan delay)
        {
            var request = new ReleaseRequest(id, priority, (int)delay.TotalSeconds);
            return SendAndGetResult(request);
        }

        Task<bool> IWorker.BuryAsync(int id, int priority)
        {
            var request = new BuryRequest(id, priority);
            return SendAndGetResult(request);
        }

        Task<bool> IWorker.TouchAsync(int id)
        {
            var request = new TouchRequest(id);
            return SendAndGetResult(request);
        }

        Task<JobStatistics> IWorker.JobStatisticsAsync(int id)
        {
            var request = new JobStatisticsRequest(id);
            return SendAndGetResult(request);
        }

        Task<TubeStatistics> IWorker.TubeStatisticsAsync(string tube)
        {
            return ((IProducer)this).TubeStatisticsAsync(tube);
        }

        #endregion

        Task<T> SendAndGetResult<T>(Request<T> request)
        {
            return SendAndGetResult(request, CancellationToken.None);
        }

        async Task<T> SendAndGetResult<T>(Request<T> request, CancellationToken cancellationToken)
        {
            var conn = _connection;
            if (conn == null) throw new ObjectDisposedException("_connection");
            await conn.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return await request.Task.ConfigureAwait(false);
        }
    }
}
