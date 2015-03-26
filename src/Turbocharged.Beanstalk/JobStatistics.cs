using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class JobStatisticsRequest : Request<JobStatistics>
    {
        public Task<JobStatistics> Task { get { return _tcs.Task; } }

        TaskCompletionSource<JobStatistics> _tcs = new TaskCompletionSource<JobStatistics>();
        int _id;

        public JobStatisticsRequest(int id)
        {
            _id = id;
        }

        public byte[] ToByteArray()
        {
            return "stats-job {0}\r\n".FormatWith(_id).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            switch (parts[0])
            {
                case "OK":
                    try
                    {
                        var bytes = Convert.ToInt32(parts[1]);
                        var buffer = new byte[bytes];
                        stream.Read(buffer, 0, bytes);
                        stream.ReadByte(); // CR
                        stream.ReadByte(); // LF

                        var yaml = YamlHelper.ParseDictionary(buffer);
                        var result = new JobStatistics();
                        foreach (var pair in yaml)
                        {
                            switch (pair.Key)
                            {
                                case "tube": result.Tube = pair.Value; break;

                                case "id": result.Id = Convert.ToInt32(pair.Value); break;
                                case "pri": result.Priority = Convert.ToInt32(pair.Value); break;
                                case "file": result.File = Convert.ToInt32(pair.Value); break;
                                case "kicks": result.Kicks = Convert.ToInt32(pair.Value); break;
                                case "buries": result.Buries = Convert.ToInt32(pair.Value); break;
                                case "reserves": result.Reserves = Convert.ToInt32(pair.Value); break;
                                case "releases": result.Releases = Convert.ToInt32(pair.Value); break;
                                case "timeouts": result.Timeouts = Convert.ToInt32(pair.Value); break;

                                case "age": result.Age = TimeSpan.FromSeconds(Convert.ToInt32(pair.Value)); break;
                                case "ttr": result.TimeToRun = TimeSpan.FromSeconds(Convert.ToInt32(pair.Value)); break;
                                case "time-left": result.TimeLeft = TimeSpan.FromSeconds(Convert.ToInt32(pair.Value)); break;

                                case "state": result.State = (JobState)Enum.Parse(typeof(JobState), pair.Value, true); break;
                            }
                        }
                        _tcs.SetResult(result);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _tcs.SetException(ex);
                        return;
                    }

                case "NOT_FOUND":
                    _tcs.SetResult(null);
                    return;

                default:
                    _tcs.SetException(new Exception("Unknown failure"));
                    Trace.Error("Unknown stats-job response: {0}", firstLine);
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }

    public class JobStatistics
    {
        /// <summary>
        /// The job ID.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The name of the tube that contains this job.
        /// </summary>
        public string Tube { get; internal set; }

        /// <summary>
        /// The state of the job.
        /// </summary>
        public JobState State { get; internal set; }

        /// <summary>
        /// The priority value set by the put, release, or bury commands.
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// The duration since the put command that created this job.
        /// </summary>
        public TimeSpan Age { get; internal set; }

        /// <summary>
        /// The duration until the server puts this job into the ready
        /// queue. This number is only meaningful if the job is reserved or
        /// delayed. If the job is reserved and this amount of time elapses
        /// before its state changes, it is considered to have timed out.
        /// </summary>
        public TimeSpan TimeLeft { get; internal set; }

        /// <summary>
        /// The duration this job may be reserved before it times out.
        /// </summary>
        public TimeSpan TimeToRun { get; internal set; }

        /// <summary>
        /// The number of the earliest binlog file containing this job.
        /// If -b wasn't used, this will be 0.
        /// </summary>
        public int File { get; internal set; }

        /// <summary>
        /// The number of times this job has been reserved.
        /// </summary>
        public int Reserves { get; internal set; }

        /// <summary>
        /// The number of times this job has timed out during a reservation.
        /// </summary>
        public int Timeouts { get; internal set; }

        /// <summary>
        /// The number of times a client has released this job from a reservation.
        /// </summary>
        public int Releases { get; internal set; }

        /// <summary>
        /// The number of times this job has been buried.
        /// </summary>
        public int Buries { get; internal set; }

        /// <summary>
        /// The number of times this job has been kicked.
        /// </summary>
        public int Kicks { get; internal set; }
    }
}
