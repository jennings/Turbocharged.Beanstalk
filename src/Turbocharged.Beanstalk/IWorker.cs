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
    public interface IWorker
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
        /// Requests that the TimeLeft for the reserved job be reset to the TimeToRun.
        /// </summary>
        Task<bool> TouchAsync(int id);
    }
}
