using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.Beanstalk.Tests
{
    public class BasicFacts
    {
        BeanstalkConnection conn;
        IConsumer cons;
        IProducer prod;

        public BasicFacts()
        {
            var hostname = ConfigurationManager.AppSettings["Hostname"];
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            conn = new BeanstalkConnection(hostname, port);
            conn.Connect();
            cons = conn.GetConsumer();
            prod = conn.GetProducer();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(255)]
        public async Task CanPutAndPeekAJob(byte data)
        {
            var id = await prod.PutAsync(new byte[] { data }, 1, 0, 10);
            var job = await prod.PeekAsync(id);
            Assert.Equal(id, job.Id);
            Assert.Equal(data, job.Data[0]);
        }

        [Fact]
        public async Task CanReserveAJob()
        {
            await prod.PutAsync(new byte[] { 2 }, 1, 0, 10);
            var job = await cons.ReserveAsync();
            Assert.NotNull(job);
        }

        [Fact]
        public async Task UseWorksCorrectly()
        {
            // Put something in a tube
            await prod.Use("default");
            await prod.PutAsync(new byte[] { 3 }, 1, 0, 10);

            // Verify an empty tube is empty
            await prod.Use("empty");
            Assert.Null(await prod.PeekAsync());

            // Verify we see it now
            await prod.Use("default");
            var job = await prod.PeekAsync();
            Assert.NotNull(job);
        }

        [Fact]
        public async Task WatchAndIgnoreWorksCorrectly()
        {
            // Put something in an ignored tube
            await prod.Use("ignored");
            await prod.PutAsync(new byte[] { 11 }, 1, 0, 10);

            // Verify we can't see it if we ignore
            await Task.WhenAll(
                cons.Watch("empty"),
                cons.Ignore("default"),
                cons.Ignore("ignored"));
            await Assert.ThrowsAnyAsync<TimeoutException>(async () => { await cons.ReserveAsync(TimeSpan.Zero); });

            // Prove we can see it if we watch the tube again
            await Task.WhenAll(
                cons.Watch("ignored"),
                cons.Ignore("empty"));
            Assert.NotNull(await cons.ReserveAsync());
        }

        [Fact]
        public async Task PeekingInvalidJobIdReturnsNull()
        {
            var unknownJobId = 2000000; // Hope it's not there
            Assert.Null(await cons.PeekAsync(unknownJobId));
            Assert.Null(await prod.PeekAsync(unknownJobId));
        }

        [Fact]
        public async Task PeekingEmptyTubeReturnsNull()
        {
            await prod.Use("empty");
            Assert.Null(await prod.PeekAsync());
            Assert.Null(await prod.PeekAsync(JobStatus.Ready));
            Assert.Null(await prod.PeekAsync(JobStatus.Delayed));
            Assert.Null(await prod.PeekAsync(JobStatus.Buried));
        }
    }
}
