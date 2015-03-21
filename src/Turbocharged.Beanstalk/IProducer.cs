using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    /// <summary>
    /// Provides methods useful for inserting jobs into Beanstalk.
    /// </summary>
    public interface IProducer : IDisposable
    {
        /// <summary>
        /// Uses the specified tube. Jobs will be inserted into the currently-used tube.
        /// </summary>
        /// <returns></returns>
        Task<string> UseAsync(string tube);

        /// <summary>
        /// Retrieves the name of the currently-used tube.
        /// </summary>
        Task<string> UsingAsync();

        /// <summary>
        /// Puts a new job into the currently-used tube.
        /// </summary>
        /// <param name="job">The job data.</param>
        /// <param name="priority">The priority of the job. Higher-priority jobs will be delivered before lower-priority jobs.</param>
        /// <param name="delay">The duration server should wait before allowing the job to be reserved.</param>
        /// <param name="timeToRun">The duration for which this job will be reserved.</param>
        /// <returns>The ID of the inserted job.</returns>
        Task<int> PutAsync(byte[] job, int priority, TimeSpan delay, TimeSpan timeToRun);

        /// <summary>
        /// Retrives a job without reserving it.
        /// </summary>
        /// <returns>A job, or null if the job was not found.</returns>
        Task<Job> PeekAsync(int id);

        /// <summary>
        /// Retrives the first job from the Ready state in the currently-used tube.
        /// </summary>
        /// <returns>A job, or null if there are no jobs in the Ready state.</returns>
        Task<Job> PeekAsync();

        /// <summary>
        /// Retrives the first job from the specified state in the currently-used tube.
        /// </summary>
        /// <returns>A job, or null if no jobs are in the specified state.</returns>
        Task<Job> PeekAsync(JobState state);

        /// <summary>
        /// Retrieves statistics about the specified tube.
        /// </summary>
        Task<TubeStatistics> TubeStatisticsAsync(string tube);
    }
}
