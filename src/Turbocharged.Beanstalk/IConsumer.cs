using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public interface IConsumer
    {
        Task<JobDescription> ReserveAsync();
        Task<JobDescription> ReserveAsync(TimeSpan timeout);
        Task<int> Watch(string tube);
        Task<int> Ignore(string tube);
        Task<JobDescription> PeekAsync(int id);
    }
}
