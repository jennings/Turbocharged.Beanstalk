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
    public interface IConsumer : IServer, IDisposable
    {
        /// <summary>
        /// Begins watching a tube.
        /// </summary>
        /// <returns>The number of tubes currently being watched.</returns>
        Task<int> WatchAsync(string tube);

        /// <summary>
        /// Ignores jobs from a tube.
        /// </summary>
        /// <returns>The number of tubes currently being watched.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when attempting to ignore the only watched tube.</exception>
        Task<int> IgnoreAsync(string tube);

        /// <summary>
        /// Returns a list of tubes currently being watched.
        /// </summary>
        Task<List<string>> ListTubesWatchedAsync();

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

        /// <summary>
        /// Retrieves a job without reserving it.
        /// </summary>
        /// <returns>A job, or null if the job was not found.</returns>
        Task<Job> PeekAsync(int id);

        /// <summary>
        /// Deletes the specified job.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the job ID is not found.</exception>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Releases the specified job so another consumer may reserve it.
        /// </summary>
        Task<bool> ReleaseAsync(int id, int priority, TimeSpan delay);

        /// <summary>
        /// Buries the specified job so no other consumers can reserve it.
        /// </summary>
        Task<bool> BuryAsync(int id, int priority);

        /// <summary>
        /// Requests that the TimeLeft for the specified reserved job be reset to the TimeToRun.
        /// </summary>
        Task<bool> TouchAsync(int id);

        /// <summary>
        /// Kicks a job into the Ready queue from the Delayed or Buried queues.
        /// </summary>
        /// <param name="id">The job ID.</param>
        /// <returns>True if the job was kicked, or false if it was not found or not in a kickable state.</returns>
        Task<bool> KickJobAsync(int id);
    }
}
