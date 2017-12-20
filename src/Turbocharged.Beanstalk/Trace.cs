using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    static class Trace
    {
        // Commented out until I find a suitable replacement in .NET Standard

        // readonly static TraceSource source = new TraceSource("Turbocharged.Beanstalk");

        public static void Info(string message)
        {
            // source.TraceInformation(message);
        }

        public static void Info(string format, params object[] args)
        {
            // source.TraceInformation(format, args);
        }

        public static void Error(string message)
        {
            // source.TraceEvent(TraceEventType.Error, 0, message);
        }

        public static void Error(string format, params object[] args)
        {
            // source.TraceEvent(TraceEventType.Error, 0, format, args);
        }

        public static void Verbose(string message)
        {
            // source.TraceEvent(TraceEventType.Verbose, 0, message);
        }

        public static void Verbose(string format, params object[] args)
        {
            // source.TraceEvent(TraceEventType.Verbose, 0, format, args);
        }
    }
}
