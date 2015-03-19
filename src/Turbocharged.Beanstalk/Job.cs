using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Turbocharged.Beanstalk
{
    public class Job
    {
        public int Id { get; internal set; }
        public byte[] Data { get; internal set; }
    }
}
