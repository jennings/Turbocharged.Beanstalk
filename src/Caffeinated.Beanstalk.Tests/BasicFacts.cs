using System;
using System.Collections.Generic;
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
            conn = new BeanstalkConnection("172.16.80.1", 11300);
            conn.Connect();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(200)]
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
    }
}
