using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class TubeStatisticsRequest : Request<TubeStatistics>
    {
        public Task<TubeStatistics> Task { get { return _tcs.Task; } }

        TaskCompletionSource<TubeStatistics> _tcs = new TaskCompletionSource<TubeStatistics>();
        Tube _tube;

        public TubeStatisticsRequest(Tube tube)
        {
            _tube = tube;
        }

        public byte[] ToByteArray()
        {
            return "stats-tube {0}\r\n".FormatWith(_tube.Name).ToASCIIByteArray();
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
                        var result = new TubeStatistics();
                        foreach (var pair in yaml)
                        {
                            switch (pair.Key)
                            {
                                case "name": result.Name = pair.Value; break;

                                case "total-jobs": result.TotalJobs = Convert.ToInt32(pair.Value); break;
                                case "current-using": result.CurrentUsing = Convert.ToInt32(pair.Value); break;
                                case "current-waiting": result.CurrentWaiting = Convert.ToInt32(pair.Value); break;
                                case "current-watching": result.CurrentWatching = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-ready": result.CurrentJobsReady = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-buried": result.CurrentJobsBuried = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-urgent": result.CurrentJobsUrgent = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-delayed": result.CurrentJobsDelayed = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-reserved": result.CurrentJobsReserved = Convert.ToInt32(pair.Value); break;
                                case "cmd-delete": result.DeleteCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-pause-tube": result.PauseCount = Convert.ToInt32(pair.Value); break;

                                case "pause": result.Pause = TimeSpan.FromSeconds(Convert.ToInt32(pair.Value)); break;
                                case "pause-time-left": result.PauseTimeLeft = TimeSpan.FromSeconds(Convert.ToInt32(pair.Value)); break;
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
                    Trace.Error("Unknown stats-tube response: {0}", firstLine);
                    _tcs.SetException(new Exception("Unknown failure"));
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }

    public class TubeStatistics
    {
        /// <summary>
        /// The tube's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of ready jobs with Priority less than 1024 in this tube.
        /// </summary>
        public int CurrentJobsUrgent { get; set; }

        /// <summary>
        /// The number of jobs in the ready queue in this tube.
        /// </summary>
        public int CurrentJobsReady { get; set; }

        /// <summary>
        /// The number of jobs reserved by all clients in this tube.
        /// </summary>
        public int CurrentJobsReserved { get; set; }

        /// <summary>
        /// The number of delayed jobs in this tube.
        /// </summary>
        public int CurrentJobsDelayed { get; set; }

        /// <summary>
        /// The number of buried jobs in this tube.
        /// </summary>
        public int CurrentJobsBuried { get; set; }

        /// <summary>
        /// The cumulative count of jobs created in this tube in the current beanstalkd process.
        /// </summary>
        public int TotalJobs { get; set; }

        /// <summary>
        /// The number of open connections that are currently using this tube.
        /// </summary>
        public int CurrentUsing { get; set; }

        /// <summary>
        /// The number of open connections that have issued a reserve command
        /// while watching this tube but not yet received a response.
        /// </summary>
        public int CurrentWaiting { get; set; }

        /// <summary>
        /// The number of open connections that are currently watching this tube.
        /// </summary>
        public int CurrentWatching { get; set; }

        /// <summary>
        /// The cumulative number of delete commands for this tube.
        /// </summary>
        public int DeleteCount { get; set; }

        /// <summary>
        /// The cumulative number of pause-tube commands for this tube.
        /// </summary>
        public int PauseCount { get; set; }

        /// <summary>
        /// The duration the tube has been paused.
        /// </summary>
        public TimeSpan Pause { get; set; }

        /// <summary>
        /// The duration until the tube is unpaused.
        /// </summary>
        public TimeSpan PauseTimeLeft { get; set; }
    }
}
