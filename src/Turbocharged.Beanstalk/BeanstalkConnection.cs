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
    public sealed class BeanstalkConnection : IProducer, IConsumer, IServer, IDisposable
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
        }

        static async Task<BeanstalkConnection> ConnectAsync(string hostname, int port)
        {
            var connection = new BeanstalkConnection(hostname, port);
            connection._connection = await PhysicalConnection.ConnectAsync(hostname, port).ConfigureAwait(false); // Yo dawg
            return connection;
        }

        /// <summary>
        /// Creates a consumer with a dedicated TCP connection to a Beanstalk server.
        /// </summary>
        public static async Task<IConsumer> ConnectConsumerAsync(string hostname, int port)
        {
            return await ConnectAsync(hostname, port).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a producer with a dedicated TCP connection to a Beanstalk server.
        /// </summary>
        public static async Task<IProducer> ConnectProducerAsync(string hostname, int port)
        {
            return await ConnectAsync(hostname, port).ConfigureAwait(false);
        }

        /// <summary>
        /// Schedules a worker with a dedicated TCP connection to repeatedly reserve jobs
        /// from the specified tubes and process them.
        /// </summary>
        /// <param name="hostname">The hostname of the Beanstalk server.</param>
        /// <param name="port">The port number of the Beanstalk server.</param>
        /// <param name="tubes">The tubes to watch.</param>
        /// <param name="worker">The delegate used to processed reserved jobs.</param>
        /// <returns>A token which stops the worker when disposed.</returns>
        public static async Task<IDisposable> ConnectWorkerAsync(string hostname, int port, WorkerOptions options, WorkerFunc worker)
        {
            // Must capture the context before the first await
            if (options.TaskScheduler == null)
                options.TaskScheduler = SynchronizationContext.Current == null
                    ? TaskScheduler.Default
                    : TaskScheduler.FromCurrentSynchronizationContext();

            var conn = await BeanstalkConnection.ConnectAsync(hostname, port).ConfigureAwait(false);
            try
            {
                // Just take the default tube if none was given
                if (options.Tubes.Count > 0)
                {
                    foreach (var tube in options.Tubes)
                        await ((IConsumer)conn).WatchAsync(tube).ConfigureAwait(false);
                    if (!options.Tubes.Contains("default"))
                        await ((IConsumer)conn).IgnoreAsync("default").ConfigureAwait(false);
                }
            }
            catch
            {
                conn.Dispose();
                throw;
            }

            var cts = new CancellationTokenSource();
            var disposable = Disposable.Create(() =>
            {
                cts.Cancel();
                cts.Dispose();
                conn.Dispose();
            });
#pragma warning disable 4014
            conn.WorkerLoop(worker, options, cts.Token)
                .ContinueWith(t => disposable.Dispose(), TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
            return disposable;
        }

        async Task WorkerLoop(WorkerFunc worker, WorkerOptions options, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var job = await ReserveAsync(cancellationToken).ConfigureAwait(false);
                if (job != null)
                {
                    try
                    {
                        // Details: http://blog.stephencleary.com/2013/08/startnew-is-dangerous.html
                        await Task.Factory.StartNew(
                                () => worker(this, job),
                                cancellationToken,
                                TaskCreationOptions.DenyChildAttach,
                                options.TaskScheduler)
                            .Unwrap()
                            .ConfigureAwait(false);
                        continue;
                    }
                    catch (Exception)
                    {
                        // Carry on outside the catch...
                    }
                    var cons = this as IConsumer;
                    int priority;
                    switch (options.FailureBehavior)
                    {
                        case WorkerFailureBehavior.Bury:
                            priority = options.FailurePriority ?? (await cons.JobStatisticsAsync(job.Id).ConfigureAwait(false)).Priority;
                            await cons.BuryAsync(job.Id, priority).ConfigureAwait(false);
                            continue;

                        case WorkerFailureBehavior.Release:
                            priority = options.FailurePriority ?? (await cons.JobStatisticsAsync(job.Id).ConfigureAwait(false)).Priority;
                            await cons.ReleaseAsync(job.Id, priority, options.FailureReleaseDelay).ConfigureAwait(false);
                            continue;

                        case WorkerFailureBehavior.Delete:
                            await cons.DeleteAsync(job.Id).ConfigureAwait(false);
                            continue;

                        case WorkerFailureBehavior.NoAction:
                            continue;

                        default:
                            throw new InvalidOperationException("Unhandled WorkerFailureBehavior '" + options.FailureBehavior.ToString() + "'");
                    }
                }
            }
        }

        /// <summary>
        /// Closes the connection to the Beanstalk server.
        /// </summary>
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

        Task<string> IProducer.ListTubeUsedAsync()
        {
            var request = new ListTubeUsedRequest();
            return SendAndGetResult(request);
        }

        Task<int> IProducer.PutAsync(byte[] job, int priority, TimeSpan timeToRun)
        {
            return ((IProducer)this).PutAsync(job, priority, timeToRun, TimeSpan.Zero);
        }

        Task<int> IProducer.PutAsync(byte[] job, int priority, TimeSpan timeToRun, TimeSpan delay)
        {
            var request = new PutRequest(job, priority, timeToRun, delay);
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

        Task<int> IProducer.KickAsync(int count)
        {
            var request = new KickRequest(count);
            return SendAndGetResult(request);
        }

        Task<bool> IProducer.KickJobAsync(int id)
        {
            var request = new KickJobRequest(id);
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

        Task<List<string>> IWorker.ListTubesWatchedAsync()
        {
            var request = new ListTubesWatchedRequest();
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

        Task<bool> IWorker.KickJobAsync(int id)
        {
            return ((IProducer)this).KickJobAsync(id);
        }

        #endregion

        #region Server

        Task<List<string>> IServer.ListTubesAsync()
        {
            var request = new ListTubesRequest();
            return SendAndGetResult(request);
        }

        Task<bool> IServer.PauseTubeAsync(string tube, TimeSpan duration)
        {
            var request = new PauseTubeRequest(tube, duration);
            return SendAndGetResult(request);
        }

        Task<JobStatistics> IServer.JobStatisticsAsync(int id)
        {
            var request = new JobStatisticsRequest(id);
            return SendAndGetResult(request);
        }

        Task<TubeStatistics> IServer.TubeStatisticsAsync(string tube)
        {
            var request = new TubeStatisticsRequest(tube);
            return SendAndGetResult(request);
        }

        Task<Statistics> IServer.ServerStatisticsAsync()
        {
            var request = new StatisticsRequest();
            return SendAndGetResult(request);
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
