using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    /// <summary>
    /// Provides methods to reserve jobs and work with the results.
    /// </summary>
    public interface IConsumer : IWorker, IDisposable
    {
        /// <summary>
        /// Reserve a job, waiting indefinitely.
        /// </summary>
        /// <returns>A reserved job, or null on a DEADLINE_SOON response.</returns>
        Task<Job> ReserveAsync();

        /// <summary>
        /// Reserve a job, waiting for the specified timeout.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>A reserved job, or null on a DEADLINE_SOON response.</returns>
        /// <exception cref="System.TimeoutException">Thrown when the timeout period elapses.</exception>
        Task<Job> ReserveAsync(TimeSpan timeout);
    }
}
