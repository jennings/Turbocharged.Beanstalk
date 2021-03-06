﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class IgnoreRequest : Request<int>
    {
        public Task<int> Task { get { return _tcs.Task; } }
        public Tube Tube { get; set; }

        TaskCompletionSource<int> _tcs = new TaskCompletionSource<int>();

        public IgnoreRequest(Tube tube)
        {
            if (tube == null) throw new ArgumentNullException("tube");
            Tube = tube;
        }

        public byte[] ToByteArray()
        {
            return "ignore {0}\r\n".FormatWith(Tube.Name)
                .ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(firstLine))
            {
                _tcs.SetException(new Exception("Empty ignore response"));
                return;
            }

            var parts = firstLine.Split(' ');
            switch (parts[0])
            {
                case "WATCHING":
                    if (parts.Length == 2)
                    {
                        var num = Convert.ToInt32(parts[1]);
                        _tcs.SetResult(num);
                    }
                    else
                    {
                        _tcs.SetException(new Exception("Unknown ignore response"));
                    }
                    return;

                case "NOT_IGNORED":
                    _tcs.SetException(new InvalidOperationException("Must be watching at least one tube"));
                    return;
            }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
