using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated.Beanstalk
{
    public interface IProducer
    {
        Task<string> Use(string tube);
        Task<int> PutAsync(byte[] job, int priority, int delay, int ttr);
        Task<JobDescription> PeekAsync(int id);
        Task<JobDescription> PeekAsync();
        Task<JobDescription> PeekAsync(JobStatus status);
    }
}
