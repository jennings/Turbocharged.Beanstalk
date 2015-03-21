using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    /// <summary>
    /// Provides methods useful for inspecting jobs and working with jobs that have
    /// already been reserved.
    /// </summary>
    public interface IWorker : IServer
    {
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
        /// Deletes the specified job.
        /// </summary>
        Task<bool> TouchAsync(int id);

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
        Task<List<string>> WatchedAsync();

        /// <summary>
        /// Retrives a job without reserving it.
        /// </summary>
        /// <returns>A job, or null if the job was not found.</returns>
        Task<Job> PeekAsync(int id);
    }
}
