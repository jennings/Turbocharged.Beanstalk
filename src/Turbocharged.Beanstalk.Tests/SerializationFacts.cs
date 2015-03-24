using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        static TimeSpan ZeroSeconds = TimeSpan.Zero;
        static TimeSpan TenSeconds = TimeSpan.FromSeconds(10);

        public SerializationFacts()
        {
            hostname = Environment.GetEnvironmentVariable("BEANSTALK_HOSTNAME") ?? ConfigurationManager.AppSettings["Hostname"];
            port = Convert.ToInt32(Environment.GetEnvironmentVariable("BEANSTALK_PORT") ?? ConfigurationManager.AppSettings["Port"]);
            connectionString = string.Format("{0}:{1}", hostname, port);
            config = new ConnectionConfiguration
            {
                Hostname = hostname,
                Port = port,
                JobSerializer = new JsonNetSerializer(),
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
        public async Task ThrowsWhenAJobSerializerIsNotProvided()
        {
            var prod = await BeanstalkConnection.ConnectProducerAsync(connectionString);
            var obj = new Jobject { };
            await Assert.ThrowsAsync<InvalidOperationException>(() => prod.PutAsync<Jobject>(obj, 1, TenSeconds));
        }

        [Fact]
        public async Task CanSerializeAJob()
        {
            await ConnectAsync();
            var obj = new Jobject { Int = 34, String = "Hello!" };
            var id = await prod.PutAsync<Jobject>(obj, 1, TimeSpan.FromSeconds(1));
            var job = await cons.PeekAsync<Jobject>(id);

            Assert.Equal(id, job.Id);
            Assert.Equal(obj.Int, job.Object.Int);
            Assert.Equal(obj.String, job.Object.String);
        }
    }

    class Jobject
    {
        public int Int { get; set; }
        public string String { get; set; }
    }

    class JsonNetSerializer : IJobSerializer
    {
        public byte[] Serialize<T>(T job)
        {
            var str = JsonConvert.SerializeObject(job);
            var bytes = Encoding.UTF8.GetBytes(str);
            return bytes;
        }

        public T Deserialize<T>(byte[] buffer)
        {
            var str = Encoding.UTF8.GetString(buffer);
            var obj = JsonConvert.DeserializeObject<T>(str);
            return obj;
        }
    }
}
