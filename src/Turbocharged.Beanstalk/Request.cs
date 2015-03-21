using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    interface Request
    {
        byte[] ToByteArray();
        void Process(string firstLine, NetworkStream stream);
        void Cancel();
    }

    interface Request<T> : Request
    {
        Task<T> Task { get; }
    }
}
