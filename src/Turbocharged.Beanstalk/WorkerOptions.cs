using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public class WorkerOptions
    {
        /// <summary>
        /// The TaskScheduler used when calling the worker delegate.
        /// If not set, the current SynchronizationContext will be captured and used.
        /// </summary>
        public TaskScheduler TaskScheduler { get; set; }

        /// <summary>
        /// The tubes this worker watches. If you do not set any, "default" will be watched automatically.
        /// </summary>
        public List<string> Tubes { get; set; }

        /// <summary>
        /// The number of workers to spawn. All workers listen on a single TCP connection.
        /// </summary>
        public int NumberOfWorkers { get; set; }

        /// <summary>
        /// The action that should be taken if a worker function
        /// throws an unhandled exception.
        /// </summary>
        public WorkerFailureBehavior FailureBehavior { get; set; }

        /// <summary>
        /// The priority to set when burying or releasing a job
        /// which resulted in an unhandled exception.
        /// </summary>
        public int? FailurePriority { get; set; }

        /// <summary>
        /// The delay to set when releasing a job which resulted
        /// in an unhandled exception.
        /// </summary>
        public TimeSpan FailureReleaseDelay { get; set; }

        public WorkerOptions()
        {
            Tubes = new List<string>();
            NumberOfWorkers = 1;
            FailureBehavior = WorkerFailureBehavior.Bury;
            FailurePriority = null;
            FailureReleaseDelay = TimeSpan.Zero;
        }
    }

    public enum WorkerFailureBehavior
    {
        Bury,
        Release,
        Delete,
        NoAction,
    }
}
