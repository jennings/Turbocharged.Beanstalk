using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public class TubeStatistics
    {
        public string Name { get; set; }
        public int CurrentJobsUrgent { get; set; }
        public int CurrentJobsReady { get; set; }
        public int CurrentJobsReserved { get; set; }
        public int CurrentJobsDelayed { get; set; }
        public int CurrentJobsBuried { get; set; }
        public int TotalJobs { get; set; }
        public int CurrentUsing { get; set; }
        public int CurrentWaiting { get; set; }
        public int CurrentWatching { get; set; }
        public int DeleteCount { get; set; }
        public int PauseCount { get; set; }
        public TimeSpan Pause { get; set; }
        public TimeSpan PauseTimeLeft  { get; set; }
    }

    class TubeStatisticsRequest : Request<TubeStatistics>
    {
        public Task<TubeStatistics> Task { get { return _tcs.Task; } }

        TaskCompletionSource<TubeStatistics> _tcs = new TaskCompletionSource<TubeStatistics>();
        string _tube;

        public TubeStatisticsRequest(string tube)
        {
            _tube = tube;
        }

        public byte[] ToByteArray()
        {
            return "stats-tube {0}\r\n".FormatWith(_tube).ToASCIIByteArray();
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
        /*
         * 
    name is the tube's name.
    current-jobs-urgent is the number of ready jobs with priority < 1024 in this tube.
    current-jobs-ready is the number of jobs in the ready queue in this tube.
    current-jobs-reserved is the number of jobs reserved by all clients in this tube.
    current-jobs-delayed is the number of delayed jobs in this tube.
    current-jobs-buried is the number of buried jobs in this tube.
    total-jobs is the cumulative count of jobs created in this tube in the current beanstalkd process.
    current-using is the number of open connections that are currently using this tube.
    current-waiting is the number of open connections that have issued a reserve command while watching this tube but not yet received a response.
    current-watching is the number of open connections that are currently watching this tube.
    pause is the number of seconds the tube has been paused for.
    cmd-delete is the cumulative number of delete commands for this tube
    cmd-pause-tube is the cumulative number of pause-tube commands for this tube.
    pause-time-left is the number of seconds until the tube is un-paused.
*/
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
                    return;
            }
        }
    }
}
