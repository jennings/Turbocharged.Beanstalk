using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Turbocharged.Beanstalk.Tests.FakesAndMocks;
using Xunit;

namespace Turbocharged.Beanstalk.Tests
{
    public class SerializationFacts : IDisposable
    {
        IConsumer cons;
        IProducer prod;
        string hostname;
        int port;
        string connectionString;
        ConnectionConfiguration config;
        WorkerOptions options;

        static TimeSpan ZeroSeconds = TimeSpan.Zero;
        static TimeSpan TenSeconds = TimeSpan.FromSeconds(10);

        public SerializationFacts()
        {
            hostname = Environment.GetEnvironmentVariable("BEANSTALK_HOSTNAME") ?? ConfigurationManager.AppSettings["Hostname"];
            port = Convert.ToInt32(Environment.GetEnvironmentVariable("BEANSTALK_PORT") ?? ConfigurationManager.AppSettings["Port"]);
            connectionString = string.Format("{0}:{1}", hostname, port);
            options = new WorkerOptions { Tubes = { "jobjects" } };
            config = new ConnectionConfiguration
            {
                Hostname = hostname,
                Port = port,
            };
        }

        public void Dispose()
        {
            if (cons != null) cons.Dispose();
            if (prod != null) prod.Dispose();
        }

        public async Task ConnectAsync()
        {
            prod = await BeanstalkConnection.ConnectProducerAsync(config);
            cons = await BeanstalkConnection.ConnectConsumerAsync(config);
            await prod.UseAsync("jobjects");
            await cons.WatchAsync("jobjects");
            await cons.IgnoreAsync("default");
        }

        async Task DrainUsedTube()
        {
            Job job;
            while ((job = await prod.PeekAsync()) != null)
                await cons.DeleteAsync(job.Id);
        }

        [Fact]
        public async Task CanSerializeAndDeserializeAJob()
        {
            await ConnectAsync();
            var obj = new Jobject { Int = 34, String = "Hello!" };
            var id = await prod.PutAsync<Jobject>(obj, 1, TimeSpan.FromSeconds(1));
            var job = await cons.PeekAsync<Jobject>(id);

            Assert.Equal(id, job.Id);
            Assert.Equal(obj.Int, job.Object.Int);
            Assert.Equal(obj.String, job.Object.String);
        }

        [Fact]
        public async Task PeekingMessagesThatCannotDeserializeThrowsDeserializationException()
        {
            byte[] badJSON = Encoding.UTF8.GetBytes(@"{ ""invalid"" : ""JSON"", ""garbage"": }}");

            await ConnectAsync();
            await DrainUsedTube();
            var id = await prod.PutAsync(badJSON, 1, TenSeconds);

            try
            {
                var job = await cons.PeekAsync<Jobject>(id);
                throw new Exception(string.Format("Peek did not throw. Job = ", job));
            }
            catch (DeserializationException ex)
            {
                Assert.Equal(ex.Job.Id, id);
                Assert.Equal(badJSON, ex.Job.Data);
            }
        }

        [Fact]
        public async Task ReservingMessagesThatCannotDeserializeThrowsDeserializationException()
        {
            byte[] badJSON = Encoding.UTF8.GetBytes(@"{ ""invalid"" : ""JSON"", ""garbage"": }}");

            await ConnectAsync();
            await DrainUsedTube();
            var id = await prod.PutAsync(badJSON, 1, TenSeconds);

            try
            {
                // Reserve succeeds, deserialization fails
                await cons.ReserveAsync<Jobject>();
            }
            catch (DeserializationException dex)
            {
                Assert.Equal(id, dex.Job.Id);
                Assert.Equal(badJSON, dex.Job.Data);
            }

            // The message should be reserved
            var stats = await prod.JobStatisticsAsync(id);
            Assert.Equal(JobState.Reserved, stats.State);

            // Clean up
            await cons.DeleteAsync(id);
        }

        [Fact]
        public async Task ReservingMessagesWithTimeoutThatCannotDeserializeThrowsDeserializationException()
        {
            byte[] badJSON = Encoding.UTF8.GetBytes(@"{ ""invalid"" : ""JSON"", ""garbage"": }}");

            await ConnectAsync();
            await DrainUsedTube();
            var id = await prod.PutAsync(badJSON, 1, TenSeconds);

            try
            {
                // Reserve succeeds, deserialization fails
                await cons.ReserveAsync<Jobject>(TimeSpan.FromSeconds(1));
            }
            catch (DeserializationException dex)
            {
                Assert.Equal(id, dex.Job.Id);
                Assert.Equal(badJSON, dex.Job.Data);
            }

            // The message should be reserved
            var stats = await prod.JobStatisticsAsync(id);
            Assert.Equal(JobState.Reserved, stats.State);

            await cons.DeleteAsync(id);
        }

        [Fact]
        public async Task TypedWorkersWork()
        {
            int counter = 0;
            var worker = BeanstalkConnection.ConnectWorkerAsync<Jobject>(config, options, async (w, j) =>
            {
                counter++;
                await w.DeleteAsync();
            });

            using (await worker)
            {
                await ConnectAsync();
                await prod.PutAsync<Jobject>(new Jobject { }, 1, TimeSpan.FromSeconds(30));
                await prod.PutAsync<Jobject>(new Jobject { }, 1, TimeSpan.FromSeconds(30));
                await prod.PutAsync<Jobject>(new Jobject { }, 1, TimeSpan.FromSeconds(30));
                await Task.Delay(200);
            }

            Assert.InRange(counter, 3, int.MaxValue);
        }

        [Fact]
        public async Task UsesCustomSerializer()
        {
            var serializer = new CountingSerializer();
            config.JobSerializer = serializer;
            var prod = await BeanstalkConnection.ConnectProducerAsync(config);
            await prod.UseAsync("jobjects");
            var id = await prod.PutAsync<Jobject>(new Jobject(), 1, TimeSpan.FromSeconds(10));
            await prod.PeekAsync<Jobject>(id);

            Assert.Equal(1, serializer.SerializeCount);
            Assert.Equal(1, serializer.DeserializeCount);
        }

        [Fact]
        public async Task SerializerExtensionsSucceedWithMockedImplementations()
        {
            var jobs = new[]
            {
                new Job(1, Encoding.Unicode.GetBytes(@"{ 'Int': 1, 'String': 'hello' }")),
                new Job(2, Encoding.Unicode.GetBytes(@"{ 'Int': 2, 'String': 'world' }")),
            };
            var fake = new FakeConsumer(jobs);

            var job = await fake.ReserveAsync<Jobject>();
            Assert.Equal(1, job.Id);
        }
    }

    class Jobject
    {
        public int Int { get; set; }
        public string String { get; set; }
    }
}
