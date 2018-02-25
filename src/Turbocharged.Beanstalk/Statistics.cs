using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class StatisticsRequest : Request<Statistics>
    {
        public Task<Statistics> Task { get { return _tcs.Task; } }

        TaskCompletionSource<Statistics> _tcs = new TaskCompletionSource<Statistics>();

        public StatisticsRequest()
        {
        }

        public byte[] ToByteArray()
        {
            return "stats\r\n".ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream, ILogger logger)
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
                        var result = new Statistics();
                        foreach (var pair in yaml)
                        {
                            switch (pair.Key)
                            {
                                case "current-jobs-urgent": result.CurrentJobsUrgent = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-ready": result.CurrentJobsReady = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-reserved": result.CurrentJobsReserved = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-delayed": result.CurrentJobsDelayed = Convert.ToInt32(pair.Value); break;
                                case "current-jobs-buried": result.CurrentJobsBuried = Convert.ToInt32(pair.Value); break;
                                case "cmd-put": result.PutCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-peek": result.PeekCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-peek-ready": result.PeekReadyCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-peek-delayed": result.PeekDelayedCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-peek-buried": result.PeekBuriedCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-reserve": result.ReserveCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-reserve-with-timeout": result.ReserveWithTimeoutCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-use": result.UseCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-watch": result.WatchCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-ignore": result.IgnoreCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-delete": result.DeleteCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-release": result.ReleaseCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-bury": result.BuryCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-kick": result.KickCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-touch": result.TouchCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-stats": result.StatsCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-stats-job": result.StatsJobCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-stats-tube": result.StatsTubeCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-list-tubes": result.ListTubesCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-list-tube-used": result.ListTubeUsedCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-list-tubes-watched": result.ListTubesWatchedCommandCount = Convert.ToInt32(pair.Value); break;
                                case "cmd-pause-tube": result.PauseTubeCommandCount = Convert.ToInt32(pair.Value); break;
                                case "job-timeouts": result.JobTimeoutCount = Convert.ToInt32(pair.Value); break;
                                case "total-jobs": result.TotalJobCount = Convert.ToInt32(pair.Value); break;
                                case "max-job-size": result.MaxJobSize = Convert.ToInt32(pair.Value); break;
                                case "current-tubes": result.CurrentTubes = Convert.ToInt32(pair.Value); break;
                                case "current-connections": result.CurrentConnections = Convert.ToInt32(pair.Value); break;
                                case "current-producers": result.CurrentProducers = Convert.ToInt32(pair.Value); break;
                                case "current-workers": result.CurrentWorkers = Convert.ToInt32(pair.Value); break;
                                case "current-waiting": result.CurrentWaiting = Convert.ToInt32(pair.Value); break;
                                case "total-connections": result.TotalConnectionCount = Convert.ToInt32(pair.Value); break;
                                case "pid": result.ProcessID = Convert.ToInt32(pair.Value); break;
                                case "uptime": result.Uptime = TimeSpan.FromSeconds(Convert.ToInt32(pair.Value)); break;
                                case "binlog-oldest-index": result.BinlogOldestIndex = Convert.ToInt32(pair.Value); break;
                                case "binlog-current-index": result.BinlogCurrentIndex = Convert.ToInt32(pair.Value); break;
                                case "binlog-max-size": result.BinlogMaxSize = Convert.ToInt32(pair.Value); break;
                                case "binlog-records-written": result.BinlogRecordsWritten = Convert.ToInt32(pair.Value); break;
                                case "binlog-records-migrated": result.BinlogRecordsMigrated = Convert.ToInt32(pair.Value); break;
                                case "rusage-utime": result.RusageUtime = Convert.ToDecimal(pair.Value); break;
                                case "rusage-stime": result.RusageStime = Convert.ToDecimal(pair.Value); break;
                                case "id": result.Id = pair.Value; break;
                                case "hostname": result.Hostname = pair.Value; break;
                                case "version": result.Version = pair.Value; break;
                            }
                        }
                        _tcs.SetResult(result);
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(0, ex, "Unhandled exception while processing {Request}", GetType().Name);
                        _tcs.SetException(ex);
                        return;
                    }

                default:
                    Reply.SetGeneralException(_tcs, firstLine, "stats", logger);
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }

    public class Statistics
    {
        /// <summary>
        /// The number of ready jobs with priority less than 1024.
        /// </summary>
        public int CurrentJobsUrgent { get; internal set; }

        /// <summary>
        /// The number of jobs in the ready queue.
        /// </summary>
        public int CurrentJobsReady { get; internal set; }

        /// <summary>
        /// The number of jobs reserved by all clients.
        /// </summary>
        public int CurrentJobsReserved { get; internal set; }

        /// <summary>
        /// The number of delayed jobs.
        /// </summary>
        public int CurrentJobsDelayed { get; internal set; }

        /// <summary>
        /// The number of buried jobs.
        /// </summary>
        public int CurrentJobsBuried { get; internal set; }

        /// <summary>
        /// The cumulative number of put commands.
        /// </summary>
        public int PutCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of peek commands.
        /// </summary>
        public int PeekCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of peek-ready commands.
        /// </summary>
        public int PeekReadyCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of peek-delayed commands.
        /// </summary>
        public int PeekDelayedCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of peek-buried commands.
        /// </summary>
        public int PeekBuriedCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of reserve commands.
        /// </summary>
        public int ReserveCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of reserve-with-timeout commands.
        /// </summary>
        public int ReserveWithTimeoutCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of use commands.
        /// </summary>
        public int UseCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of watch commands.
        /// </summary>
        public int WatchCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of ignore commands.
        /// </summary>
        public int IgnoreCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of delete commands.
        /// </summary>
        public int DeleteCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of release commands.
        /// </summary>
        public int ReleaseCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of bury commands.
        /// </summary>
        public int BuryCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of stats-tube commands.
        /// </summary>
        public int TouchCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of kick commands.
        /// </summary>
        public int KickCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of stats commands.
        /// </summary>
        public int StatsCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of stats-job commands.
        /// </summary>
        public int StatsJobCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of stats-tube commands.
        /// </summary>
        public int StatsTubeCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of list-tubes commands.
        /// </summary>
        public int ListTubesCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of list-tube-used commands.
        /// </summary>
        public int ListTubeUsedCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of list-tubes-watched commands.
        /// </summary>
        public int ListTubesWatchedCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of pause-tube commands.
        /// </summary>
        public int PauseTubeCommandCount { get; internal set; }

        /// <summary>
        /// The cumulative number of times a job has timed out.
        /// </summary>
        public int JobTimeoutCount { get; internal set; }

        /// <summary>
        /// The cumulative number of jobs created.
        /// </summary>
        public int TotalJobCount { get; internal set; }

        /// <summary>
        /// The maximum number of bytes in a job.
        /// </summary>
        public int MaxJobSize { get; internal set; }

        /// <summary>
        /// The number of currently-existing tubes.
        /// </summary>
        public int CurrentTubes { get; internal set; }

        /// <summary>
        /// The number of currently open connections.
        /// </summary>
        public int CurrentConnections { get; internal set; }

        /// <summary>
        /// The number of open connections that have each issued at least one put command.
        /// </summary>
        public int CurrentProducers { get; internal set; }

        /// <summary>
        /// The number of open connections that have each issued at least one reserve command.
        /// </summary>
        public int CurrentWorkers { get; internal set; }

        /// <summary>
        /// The number of open connections that have each issued a reserve command but not yet received a response.
        /// </summary>
        public int CurrentWaiting { get; internal set; }

        /// <summary>
        /// The cumulative number of connections.
        /// </summary>
        public int TotalConnectionCount { get; internal set; }

        /// <summary>
        /// The process ID of the server.
        /// </summary>
        public int ProcessID { get; internal set; }

        /// <summary>
        /// The version string of the server.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// rusage-utime is the cumulative user CPU time of this process in seconds and microseconds.
        /// </summary>
        public decimal RusageUtime { get; internal set; }

        /// <summary>
        /// rusage-stime is the cumulative system CPU time of this process in seconds and microseconds.
        /// </summary>
        public decimal RusageStime { get; internal set; }

        /// <summary>
        /// uptime is the number of seconds since this server process started running.
        /// </summary>
        public TimeSpan Uptime { get; internal set; }

        /// <summary>
        /// binlog-oldest-index is the index of the oldest binlog file needed to store the current jobs.
        /// </summary>
        public int BinlogOldestIndex { get; internal set; }

        /// <summary>
        /// binlog-current-index is the index of the current binlog file being written to. If binlog is not active this value will be 0.
        /// </summary>
        public int BinlogCurrentIndex { get; internal set; }

        /// <summary>
        /// binlog-max-size is the maximum size in bytes a binlog file is allowed to get before a new binlog file is opened.
        /// </summary>
        public int BinlogMaxSize { get; internal set; }

        /// <summary>
        /// binlog-records-written is the cumulative number of records written to the binlog.
        /// </summary>
        public int BinlogRecordsWritten { get; internal set; }

        /// <summary>
        /// binlog-records-migrated is the cumulative number of records written as part of compaction.
        /// </summary>
        public int BinlogRecordsMigrated { get; internal set; }

        /// <summary>
        /// id is a random id string for this server process, generated when each beanstalkd process starts.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// hostname is the hostname of the machine as determined by uname.
        /// </summary>
        public string Hostname { get; internal set; }
    }
}
