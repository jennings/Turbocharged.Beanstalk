using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Caffeinated.Beanstalk.Tests
{
    public class BasicFacts
    {
        BeanstalkConnection conn;

        public BasicFacts()
        {
            var hostname = ConfigurationManager.AppSettings["Hostname"];
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            conn = new BeanstalkConnection(hostname, port);
            conn.Connect();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(255)]
        public async Task CanPutAndPeekAJob(byte data)
        {
            var id = await conn.PutAsync(new byte[] { data }, 1, 0, 10);
            var job = await conn.PeekAsync(id);
            Assert.Equal(id, job.Id);
            Assert.Equal(data, job.JobData[0]);
        }

        [Fact]
        public async Task CanReserveAJob()
        {
            await conn.PutAsync(new byte[] { 2 }, 1, 0, 10);
            var job = await conn.ReserveAsync();
            Assert.NotNull(job);
        }

        [Fact]
        public async Task UseWorksCorrectly()
        {
            await conn.Use("default");
            await conn.PutAsync(new byte[] { 11 }, 1, 0, 10);
            await conn.Use("empty");
            await Assert.ThrowsAnyAsync<Exception>(async () => { await conn.PeekAsync(JobStatus.Ready); });
            await conn.Use("default");
            Assert.NotNull(await conn.PeekAsync(JobStatus.Ready));
        }
    }
}
