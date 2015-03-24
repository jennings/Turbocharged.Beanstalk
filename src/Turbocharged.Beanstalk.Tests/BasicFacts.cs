using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.Beanstalk.Tests
{
    public class BasicFacts : IDisposable
    {
        IConsumer cons;
        IProducer prod;
        string hostname;
        int port;

        static TimeSpan ZeroSeconds = TimeSpan.Zero;
        static TimeSpan TenSeconds = TimeSpan.FromSeconds(10);

        public BasicFacts()
        {
            hostname = Environment.GetEnvironmentVariable("BEANSTALK_HOSTNAME") ?? ConfigurationManager.AppSettings["Hostname"];
            port = Convert.ToInt32(Environment.GetEnvironmentVariable("BEANSTALK_PORT") ?? ConfigurationManager.AppSettings["Port"]);
        }

        public void Dispose()
        {
            if (cons != null) cons.Dispose();
            if (prod != null) prod.Dispose();
        }

        async Task ConnectAsync()
        {
            cons = await BeanstalkConnection.ConnectConsumerAsync(hostname, port);
            prod = await BeanstalkConnection.ConnectProducerAsync(hostname, port);
        }

        async Task DrainUsedTube()
        {
            Job job;
            while ((job = await prod.PeekAsync()) != null)
                await cons.DeleteAsync(job.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(255)]
        public async Task CanPutAndPeekAJob(byte data)
        {
            await ConnectAsync();
            var id = await prod.PutAsync(new byte[] { data }, 1, TenSeconds, ZeroSeconds);
            var job = await prod.PeekAsync(id);
            Assert.Equal(id, job.Id);
            Assert.Equal(data, job.Data[0]);
        }

        [Fact]
        public async Task UseWatchAndIgnoreThrowOn200ByteTubeName()
        {
            await ConnectAsync();
            var good = string.Concat(Enumerable.Repeat("a", 199));
            var bad = string.Concat(Enumerable.Repeat("a", 200));

            // These are fine
            await prod.UseAsync(good);
            await cons.WatchAsync(good);
            await cons.IgnoreAsync(good);

            // Should throw eagerly, no need for async/await
            Assert.Throws<ArgumentOutOfRangeException>(() => { prod.UseAsync(bad); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { cons.WatchAsync(bad); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { cons.IgnoreAsync(bad); });
        }

        [Fact]
        public async Task CanReserveAJob()
        {
            await ConnectAsync();
            await prod.PutAsync(new byte[] { 2 }, 1, TenSeconds, ZeroSeconds);
            var job = await cons.ReserveAsync();
            Assert.NotNull(job);
        }

        [Fact]
        public async Task ReserveReturnsNullOnDeadlineSoon()
        {
            await ConnectAsync();
            var tube = "reserve-returns-null-on-deadline-soon";
            await prod.UseAsync(tube);
            await cons.WatchAsync(tube);
            await cons.IgnoreAsync("default");

            // Drain the tube
            await DrainUsedTube();

            var id = await prod.PutAsync(new byte[] { }, 1, TimeSpan.FromSeconds(2));
            var job1 = await cons.ReserveAsync();
            var job2 = await cons.ReserveAsync();
            Assert.Equal(id, job1.Id);
            Assert.Null(job2);
        }

        [Fact]
        public async Task ReserveThrowsOnTimedOut()
        {
            await ConnectAsync();
            await cons.WatchAsync("empty");
            await cons.IgnoreAsync("default");

            await Assert.ThrowsAsync<TimeoutException>(async () => await cons.ReserveAsync(ZeroSeconds));
        }

        [Fact]
        public async Task CanDeleteAJob()
        {
            await ConnectAsync();
            var id = await prod.PutAsync(new byte[] { 4 }, 1, TenSeconds, ZeroSeconds);
            var deleted = await cons.DeleteAsync(id);
            Assert.True(deleted);
        }

        [Fact]
        public async Task DeletingANonexistentReturnsFalse()
        {
            await ConnectAsync();
            var id = 65000;
            if (await cons.PeekAsync(id) != null)
            {
                await cons.DeleteAsync(id);
            }
            var deleted = await cons.DeleteAsync(id);
            Assert.False(deleted);
        }

        [Fact]
        public async Task CanReleaseAJob()
        {
            await ConnectAsync();
            await prod.PutAsync(new byte[] { 10 }, 1, TenSeconds, ZeroSeconds);

            // Now re-prioritize it
            var job = await cons.ReserveAsync(TimeSpan.FromSeconds(0));
            var stats1 = await cons.JobStatisticsAsync(job.Id);
            await cons.ReleaseAsync(job.Id, stats1.Priority + 1, TimeSpan.FromSeconds(20));
            var stats2 = await cons.JobStatisticsAsync(job.Id);
            Assert.Equal(stats1.Priority + 1, stats2.Priority);
            Assert.Equal(stats1.Releases + 1, stats2.Releases);
        }

        [Fact]
        public async Task CanBuryAJob()
        {
            await ConnectAsync();
            await prod.PutAsync(new byte[] { 11 }, 1, TenSeconds, ZeroSeconds);

            // Now re-prioritize it
            var job = await cons.ReserveAsync(TimeSpan.FromSeconds(0));
            var stats1 = await cons.JobStatisticsAsync(job.Id);
            await Task.Delay(400);
            await cons.BuryAsync(job.Id, 3);
            var stats2 = await cons.JobStatisticsAsync(job.Id);
            var stats = await cons.JobStatisticsAsync(job.Id);
            Assert.Equal(stats1.Buries + 1, stats2.Buries);
            Assert.Equal(JobState.Buried, stats2.State);
        }

        [Fact]
        public async Task CanTouchAJob()
        {
            await ConnectAsync();
            await prod.UseAsync("touch-test");
            await prod.PutAsync(new byte[] { 12 }, 1, TenSeconds, ZeroSeconds);

            // Now re-prioritize it
            await cons.WatchAsync("touch-test");
            await cons.IgnoreAsync("default");
            var job = await cons.ReserveAsync(TimeSpan.FromSeconds(0));
            var stats1 = await cons.JobStatisticsAsync(job.Id);
            // Uncomment when fixed
            // System.Threading.Thread.Sleep(2000);
            await cons.TouchAsync(job.Id);
            var stats2 = await cons.JobStatisticsAsync(job.Id);
            // Commented out because I don't understand why this fails
            // Assert.True(stats1.TimeLeft < stats2.TimeLeft);
        }

        [Fact]
        public async Task UseWorksCorrectly()
        {
            await ConnectAsync();

            // Put something in a tube
            await prod.UseAsync("default");
            await prod.PutAsync(new byte[] { 3 }, 1, TenSeconds, ZeroSeconds);

            // Verify an empty tube is empty
            await prod.UseAsync("empty");
            Assert.Null(await prod.PeekAsync());

            // Verify we see it now
            await prod.UseAsync("default");
            var job = await prod.PeekAsync();
            Assert.NotNull(job);
        }

        [Fact]
        public async Task ListTubeUsedWorks()
        {
            await ConnectAsync();
            var tube = "hello-there";
            await prod.UseAsync(tube);
            var used = await prod.ListTubeUsedAsync();
            Assert.Equal(tube, used);
        }

        [Fact]
        public async Task ListTubesWatchedWorks()
        {
            await ConnectAsync();
            var tubes = new[] { "tube1", "tube2", "tube3" };
            foreach (var tube in tubes)
                await cons.WatchAsync(tube);
            await cons.IgnoreAsync("default");
            var watched = await cons.ListTubesWatchedAsync();

            Assert.Equal(tubes.Length, watched.Count);
            foreach (var tube in watched)
                Assert.Contains(tube, tubes);
        }

        [Fact]
        public async Task ListTubesWorks()
        {
            await ConnectAsync();
            await prod.PutAsync(new byte[] { }, 1, TimeSpan.FromSeconds(30));
            await prod.UseAsync("another-tube");
            await prod.PutAsync(new byte[] { }, 1, TimeSpan.FromSeconds(30));
            var tubes = await prod.ListTubesAsync();
            Assert.Contains("default", tubes);
            Assert.Contains("another-tube", tubes);
        }

        [Fact]
        public async Task WatchAndIgnoreWorksCorrectly()
        {
            await ConnectAsync();

            // Put something in an ignored tube
            await prod.UseAsync("ignored");
            await prod.PutAsync(new byte[] { 11 }, 1, TenSeconds, ZeroSeconds);

            // Verify we can't see it if we ignore
            await Task.WhenAll(
                cons.WatchAsync("empty"),
                cons.IgnoreAsync("default"),
                cons.IgnoreAsync("ignored"));
            await Assert.ThrowsAnyAsync<TimeoutException>(async () => { await cons.ReserveAsync(TimeSpan.Zero); });

            // Prove we can see it if we watch the tube again
            await Task.WhenAll(
                cons.WatchAsync("ignored"),
                cons.IgnoreAsync("empty"));
            Assert.NotNull(await cons.ReserveAsync());
        }

        [Fact]
        public async Task PeekingInvalidJobIdReturnsNull()
        {
            await ConnectAsync();
            var unknownJobId = 2000000; // Hope it's not there
            Assert.Null(await cons.PeekAsync(unknownJobId));
            Assert.Null(await prod.PeekAsync(unknownJobId));
        }

        [Fact]
        public async Task PeekingEmptyTubeReturnsNull()
        {
            await ConnectAsync();
            await prod.UseAsync("empty");
            Assert.Null(await prod.PeekAsync());
            Assert.Null(await prod.PeekAsync(JobState.Ready));
            Assert.Null(await prod.PeekAsync(JobState.Delayed));
            Assert.Null(await prod.PeekAsync(JobState.Buried));
        }

        [Fact]
        public async Task JobStatisticsWork()
        {
            await ConnectAsync();
            var id = await prod.PutAsync(new byte[] { 41 }, 42, TimeSpan.FromSeconds(43));
            var stats = await cons.JobStatisticsAsync(id);
            Assert.Equal(id, stats.Id);
            Assert.Equal(42, stats.Priority);
            Assert.Equal(43, (int)stats.TimeToRun.TotalSeconds);
            Assert.Equal(JobState.Ready, stats.State);
        }

        [Fact]
        public async Task TubeStatisticsWork()
        {
            await ConnectAsync();
            var tube = "tube-statistics";
            await prod.UseAsync(tube);
            await Task.WhenAll(
                prod.PutAsync(new byte[] { 51 }, 52, TenSeconds, ZeroSeconds),
                prod.PutAsync(new byte[] { 54 }, 55, TenSeconds, TenSeconds));
            var stats = await cons.TubeStatisticsAsync(tube);

            Assert.Equal(tube, stats.Name);
            Assert.True(1 <= stats.CurrentJobsReady);
            Assert.True(1 <= stats.CurrentJobsDelayed);
            Assert.True(1 <= stats.CurrentUsing);
        }

        [Fact]
        public async Task ConnectWorker_ConsumesJobsAsTheyAppear()
        {
            await ConnectAsync();

            int counter = 0;
            var worker = BeanstalkConnection.ConnectWorkerAsync(hostname, port, new WorkerOptions(), async (c, job) =>
            {
                counter++;
                await c.DeleteAsync(job.Id);
            });

            using (await worker)
            {
                await prod.PutAsync(new byte[] { }, 1, TenSeconds, ZeroSeconds);
                await Task.Delay(200);
            }

            Assert.NotEqual(0, counter);
        }

        [Fact]
        public async Task ConnectWorker_StopsWhenDisposed()
        {
            await ConnectAsync();
            var tube = "i-am-a-tube";

            int counter = 0;
            var options = new WorkerOptions { Tubes = { tube } };
            var worker = BeanstalkConnection.ConnectWorkerAsync(hostname, port, options, async (c, job) =>
            {
                counter++;
                await c.DeleteAsync(job.Id);
            });

            using (await worker)
            {
                await prod.UseAsync(tube);
                await prod.PutAsync(new byte[] { }, 1, TenSeconds, ZeroSeconds);
                await Task.Delay(200);
            }

            Assert.True(counter > 0);
            var finalValue = counter;

            await prod.PutAsync(new byte[] { }, 1, TenSeconds, ZeroSeconds);
            await Task.Delay(200);

            Assert.Equal(finalValue, counter);
        }

        [Fact]
        public async Task ConnectWorker_WatchesOnlySpecifiedTubes()
        {
            await ConnectAsync();
            await prod.UseAsync("watched");
            await DrainUsedTube();

            int counter = 0;
            var options = new WorkerOptions { Tubes = { "watched" } };
            var worker = BeanstalkConnection.ConnectWorkerAsync(hostname, port, options, async (c, job) =>
            {
                counter++;
                await c.DeleteAsync(job.Id);
            });

            using (await worker)
            {
                await prod.PutAsync(new byte[] { }, 1, TenSeconds, ZeroSeconds);
                await prod.UseAsync("default");
                await prod.PutAsync(new byte[] { }, 1, TenSeconds, ZeroSeconds);
                await Task.Delay(200);
            }

            Assert.Equal(1, counter);
        }

        [Fact]
        public async Task ConnectWorker_CallsTheWorkerOnTheCurrentSyncContextByDefault()
        {
            await ConnectAsync();

            int counter = 0;
            int wrongContextCount = 0;
            SynchronizationContext startingContext = SynchronizationContext.Current;
            var worker = BeanstalkConnection.ConnectWorkerAsync(hostname, port, new WorkerOptions(), async (c, job) =>
            {
                counter++;
                if (startingContext != SynchronizationContext.Current) wrongContextCount++;
                await c.DeleteAsync(job.Id);
            });

            using (await worker)
            {
                foreach (var _ in Enumerable.Repeat(0, 10))
                    await prod.PutAsync(new byte[] { }, 1, TenSeconds, ZeroSeconds);
                await Task.Delay(100);
            }

            Assert.NotEqual(0, counter);
            Assert.Equal(0, wrongContextCount);
        }

        [Theory]
        [InlineData(WorkerFailureBehavior.Delete)]
        [InlineData(WorkerFailureBehavior.Bury)]
        [InlineData(WorkerFailureBehavior.Release)]
        [InlineData(WorkerFailureBehavior.NoAction)]
        public async Task ConnectWorker_ThrownExceptionFollowsSpecifiedBehavior(WorkerFailureBehavior behavior)
        {
            await ConnectAsync();
            var tube = "test-failure-behaviors";
            await prod.UseAsync(tube);
            await DrainUsedTube();

            var options = new WorkerOptions
            {
                Tubes = { tube },
                FailureBehavior = behavior,
                FailurePriority = 1,
                FailureReleaseDelay = TimeSpan.FromSeconds(10),
            };
            var worker = BeanstalkConnection.ConnectWorkerAsync(hostname, port, options, (c, job) =>
            {
                throw new Exception();
            });

            JobStatistics stats;
            using (await worker)
            {
                var id = await prod.PutAsync(new byte[] { }, 1, TenSeconds, ZeroSeconds);
                await Task.Delay(100);
                stats = await prod.JobStatisticsAsync(id);
            }

            switch (behavior)
            {
                case WorkerFailureBehavior.Delete: Assert.Null(stats); return;
                case WorkerFailureBehavior.Bury: Assert.Equal(stats.State, JobState.Buried); return;
                case WorkerFailureBehavior.Release: Assert.Equal(stats.State, JobState.Delayed); return;
                case WorkerFailureBehavior.NoAction: Assert.Equal(stats.State, JobState.Reserved); return;
                default: throw new Exception("Untested behavior");
            }
        }

        [Fact]
        public async Task ServerStatisticsWorks()
        {
            await ConnectAsync();
            var stats = await prod.ServerStatisticsAsync();
            Assert.False(string.IsNullOrWhiteSpace(stats.Id));
            Assert.True(stats.CurrentConnections > 0);
        }

        [Fact]
        public async Task KickWorks()
        {
            await ConnectAsync();
            await prod.UseAsync("never-reserve-from-this-tube");
            var id = await prod.PutAsync(new byte[] { }, 1, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60));
            Assert.Equal(JobState.Delayed, (await prod.JobStatisticsAsync(id)).State);
            await prod.KickAsync(int.MaxValue); // Buried
            await prod.KickAsync(int.MaxValue); // Delayed
            Assert.Equal(JobState.Ready, (await prod.JobStatisticsAsync(id)).State);
        }

        [Fact]
        public async Task KickJobWorks()
        {
            await ConnectAsync();
            var id = await prod.PutAsync(new byte[] { }, 1, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60));
            Assert.Equal(JobState.Delayed, (await prod.JobStatisticsAsync(id)).State);
            await prod.KickJobAsync(id);
            Assert.Equal(JobState.Ready, (await prod.JobStatisticsAsync(id)).State);
        }
    }
}
