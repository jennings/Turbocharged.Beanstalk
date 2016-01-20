using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public class Job
    {
        public int Id { get; private set; }
        public byte[] Data { get; private set; }

        public Job(int id, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Id = id;
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("Job Id = {0}, Length = {1}", Id, Data == null ? 0 : Data.Length);
        }
    }
}
