using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated.Beanstalk
{
    public enum JobStatus
    {
        Ready,
        Buried,
        Reserved,
        Delayed,
    }
}
