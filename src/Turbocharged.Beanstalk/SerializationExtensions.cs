using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public sealed class Job<T> : Job
    {
        public T Object { get; internal set; }
    }

    public static class SerializationExtensions
    {
        #region PutAsync

        public static Task<int> PutAsync<T>(this IProducer producer, T job, int priority, TimeSpan timeToRun)
        {
            var connection = producer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("producer", "Producer must have been created by Turbocharged.Beanstalk");

            var bytes = Serialize<T>(connection, job);
            return producer.PutAsync(bytes, priority, timeToRun);
        }

        public static Task<int> PutAsync<T>(this IProducer producer, T job, int priority, TimeSpan timeToRun, TimeSpan delay)
        {
            var connection = producer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("producer", "Producer must have been created by Turbocharged.Beanstalk");

            var bytes = Serialize<T>(connection, job);
            return producer.PutAsync(bytes, priority, timeToRun, delay);
        }

        #endregion

        #region ReserveAsync

        public static async Task<Job<T>> ReserveAsync<T>(this IConsumer consumer)
        {
            var connection = consumer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("consumer", "Consumer must have been created by Turbocharged.Beanstalk");

            var job = await consumer.ReserveAsync().ConfigureAwait(false);
            return Deserialize<T>(connection, job);
        }

        public static async Task<Job<T>> ReserveAsync<T>(this IConsumer consumer, TimeSpan timeout)
        {
            var connection = consumer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("consumer", "Consumer must have been created by Turbocharged.Beanstalk");

            var job = await consumer.ReserveAsync(timeout).ConfigureAwait(false);
            return Deserialize<T>(connection, job);
        }

        #endregion

        #region PeekAsync

        public static async Task<Job<T>> PeekAsync<T>(this IConsumer consumer, int id)
        {
            var connection = consumer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("consumer", "Consumer must have been created by Turbocharged.Beanstalk");

            var job = await consumer.PeekAsync(id).ConfigureAwait(false);
            return Deserialize<T>(connection, job);
        }

        public static async Task<Job<T>> PeekAsync<T>(this IProducer producer, int id)
        {
            var connection = producer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("producer", "Producer must have been created by Turbocharged.Beanstalk");

            var job = await producer.PeekAsync(id).ConfigureAwait(false);
            return Deserialize<T>(connection, job);
        }

        public static async Task<Job<T>> PeekAsync<T>(this IProducer producer)
        {
            var connection = producer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("producer", "Producer must have been created by Turbocharged.Beanstalk");

            var job = await producer.PeekAsync().ConfigureAwait(false);
            return Deserialize<T>(connection, job);
        }

        public static async Task<Job<T>> PeekAsync<T>(this IProducer producer, JobState state)
        {
            var connection = producer as BeanstalkConnection;
            if (connection == null) throw new ArgumentException("producer", "Producer must have been created by Turbocharged.Beanstalk");

            var job = await producer.PeekAsync(state).ConfigureAwait(false);
            return Deserialize<T>(connection, job);
        }

        #endregion

        static byte[] Serialize<T>(BeanstalkConnection connection, T obj)
        {
            return connection.Configuration.JobSerializer.Serialize<T>(obj);
        }

        static Job<T> Deserialize<T>(BeanstalkConnection connection, Job job)
        {
            if (job == null)
                return null;

            var obj = connection.Configuration.JobSerializer.Deserialize<T>(job.Data);
            return new Job<T>
            {
                Id = job.Id,
                Data = job.Data,
                Object = obj,
            };
        }
    }
}
