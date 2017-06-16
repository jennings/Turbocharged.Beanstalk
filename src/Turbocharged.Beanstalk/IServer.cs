using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    /// <summary>
    /// Provides methods useful for inspecting the status of a Beanstalk
    /// server.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// The configuration used to create this Beanstalk server.
        /// </summary>
        ConnectionConfiguration Configuration { get; }

        /// <summary>
        /// Retrieves the list of tubes on the server.
        /// </summary>
        Task<List<string>> ListTubesAsync();

        /// <summary>
        /// Delays new jobs from being reserved from a tube for the specified duration.
        /// </summary>
        Task<bool> PauseTubeAsync(string tube, TimeSpan duration);

        /// <summary>
        /// Retrieves statistics about the connected server.
        /// </summary>
        Task<Statistics> ServerStatisticsAsync();

        /// <summary>
        /// Returns statistics about a specified job.
        /// </summary>
        Task<JobStatistics> JobStatisticsAsync(int id);

        /// <summary>
        /// Retrieves statistics about the specified tube.
        /// </summary>
        Task<TubeStatistics> TubeStatisticsAsync(string tube);
    }
}
