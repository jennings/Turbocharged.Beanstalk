﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class ListTubeUsedRequest : Request<string>
    {
        public Task<string> Task { get { return _tcs.Task; } }

        TaskCompletionSource<string> _tcs = new TaskCompletionSource<string>();

        public byte[] ToByteArray()
        {
            return LIST_TUBE_USED;
        }

        static readonly byte[] LIST_TUBE_USED = "list-tube-used\r\n".ToASCIIByteArray();

        public void Process(string firstLine, NetworkStream stream, ILogger logger)
        {
            var parts = firstLine.Split(' ');
            if (parts.Length == 2 && parts[0] == "USING")
            {
                _tcs.SetResult(parts[1]);
            }
            else
            {
                Reply.SetGeneralException(_tcs, firstLine, "list-tube-used", logger);
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
