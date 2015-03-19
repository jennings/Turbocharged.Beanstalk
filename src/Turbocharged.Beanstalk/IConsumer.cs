﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public interface IConsumer
    {
        /// <summary>
        /// Reserve a job, waiting indefinitely.
        /// </summary>
        /// <returns>A reserved job.</returns>
        Task<JobDescription> ReserveAsync();

        /// <summary>
        /// Reserve a job, waiting for the specified timeout.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>A reserved job, or null if the timeout elapses.</returns>
        Task<JobDescription> ReserveAsync(TimeSpan timeout);

        /// <summary>
        /// Begins watching a tube.
        /// </summary>
        /// <returns>The number of tubes currently being watched.</returns>
        Task<int> Watch(string tube);

        /// <summary>
        /// Ignores jobs from a tube.
        /// </summary>
        /// <returns>The number of tubes currently being watched.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when attempting to ignore the only watched tube.</exception>
        Task<int> Ignore(string tube);

        /// <summary>
        /// Retrives a job without reserving it.
        /// </summary>
        /// <returns>A job, or null if the job was not found.</returns>
        Task<JobDescription> PeekAsync(int id);
    }
}
