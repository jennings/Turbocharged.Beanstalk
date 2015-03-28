using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    static class Reply
    {
        public static void SetGeneralException<T>(TaskCompletionSource<T> tcs, string firstLine, string commandName)
        {
            switch (firstLine)
            {
                case "OUT_OF_MEMORY":
                    tcs.SetException(new InvalidOperationException("OUT_OF_MEMORY"));
                    break;

                case "INTERNAL_ERROR":
                    tcs.SetException(new InvalidOperationException("INTERNAL_ERROR"));
                    break;

                case "BAD_FORMAT":
                    tcs.SetException(new InvalidOperationException("BAD_FORMAT"));
                    break;

                case "UNKNOWN_COMMAND":
                    Trace.Error("Unknown {0} response: {1}", commandName, firstLine);
                    tcs.SetException(new InvalidOperationException("UNKNOWN_COMMAND"));
                    break;

                default:
                    Trace.Error("Unknown {0} response: {1}", commandName, firstLine);
                    tcs.SetException(new InvalidOperationException("Unknown {0} response: {1}".FormatWith(commandName, firstLine)));
                    break;
            }
        }
    }
}
