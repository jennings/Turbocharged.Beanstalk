using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    /// <summary>
    /// Provides methods useful for  working with a job that has already been reserved.
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Deletes the reserved job.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the job ID is not found.</exception>
        Task<bool> DeleteAsync();

        /// <summary>
        /// Releases the reserved job so another consumer may reserve it.
        /// </summary>
        Task<bool> ReleaseAsync(int priority, TimeSpan delay);

        /// <summary>
        /// Buries the reserved job so no other consumers can reserve it.
        /// </summary>
        Task<bool> BuryAsync(int priority);

        /// <summary>
        /// Requests that the TimeLeft for the reserved job be reset to the TimeToRun.
        /// </summary>
        Task<bool> TouchAsync();
    }
}
