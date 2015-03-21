using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public class JobStatistics
    {
        public int Id { get; internal set; }
        public string Tube { get; internal set; }
        public JobState State { get; internal set; }
        public int Priority { get; internal set; }
        public TimeSpan Age { get; internal set; }
        public TimeSpan TimeLeft { get; internal set; }
        public TimeSpan TimeToRun { get; internal set; }
        public int File { get; internal set; }
        public int Reserves { get; internal set; }
        public int Timeouts { get; internal set; }
        public int Releases { get; internal set; }
        public int Buries { get; internal set; }
        public int Kicks { get; internal set; }
    }

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
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
