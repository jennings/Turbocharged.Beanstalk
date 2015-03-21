using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public sealed class Job
    {
        public int Id { get; internal set; }
        public byte[] Data { get; internal set; }

        public override string ToString()
        {
            return string.Format("Job Id = {0}, Length = {1}", Id, Data == null ? 0 : Data.Length);
        }
    }
}
