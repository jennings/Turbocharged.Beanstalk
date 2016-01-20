using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk.Tests
{
    class FakeConsumer : IConsumer
    {
        Queue<Job> _jobs;

        public ConnectionConfiguration Configuration { get; set; }

        public FakeConsumer(IEnumerable<Job> jobsToReturn)
        {
            _jobs = new Queue<Job>(jobsToReturn);
            if (_jobs.Count == 0)
                throw new ArgumentException("Must supply jobs");

            Configuration = new ConnectionConfiguration();
        }

        public Task<int> WatchAsync(string tube)
        {
            return Task.FromResult(1);
        }

        public Task<int> IgnoreAsync(string tube)
        {
            return Task.FromResult(1);
        }

        public Task<List<string>> ListTubesWatchedAsync()
        {
            return Task.FromResult(new List<string>() { "default" });
        }

        public Task<Job> ReserveAsync()
        {
            return Task.FromResult(GetJob());
        }

        public Task<Job> ReserveAsync(TimeSpan timeout)
        {
            return Task.FromResult(GetJob());
        }

        public Task<Job> PeekAsync(int id)
        {
            return Task.FromResult(GetJob());
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ReleaseAsync(int id, int priority, TimeSpan delay)
        {
            return Task.FromResult(true);
        }

        public Task<bool> BuryAsync(int id, int priority)
        {
            return Task.FromResult(true);
        }

        public Task<bool> TouchAsync(int id)
        {
            return Task.FromResult(true);
        }

        public Task<bool> KickJobAsync(int id)
        {
            return Task.FromResult(true);
        }

        public Task<List<string>> ListTubesAsync()
        {
            return Task.FromResult(new List<string>() { "default" });
        }

        public Task<bool> PauseTubeAsync(string tube, TimeSpan duration)
        {
            return Task.FromResult(true);
        }

        public Task<Statistics> ServerStatisticsAsync()
        {
            return Task.FromResult(new Statistics());
        }

        public Task<JobStatistics> JobStatisticsAsync(int id)
        {
            return Task.FromResult(new JobStatistics());
        }

        public Task<TubeStatistics> TubeStatisticsAsync(string tube)
        {
            return Task.FromResult(new TubeStatistics());
        }

        public void Dispose()
        {
        }

        Job GetJob()
        {
            var job = _jobs.Dequeue();
            _jobs.Enqueue(job);
            return job;
        }
    }
}
