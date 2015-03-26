using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class Tube
    {
        // It looks like the maximum length is supposed to be 200,
        // but when I run exactly 200 characters, instead of either
        // "USING <tube>" or "BAD_FORMAT", I get "INTERNAL_ERROR".
        //
        // I think we can safely block 200 character tubes for now.
        public const int MAX_TUBE_NAME_LENGTH = 199;

        static Regex namePattern = new Regex("^[a-zA-Z0-9+/;.$_()][a-zA-Z0-9+/;.$_()-]*$", RegexOptions.Compiled);

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public Tube(string tube)
        {
            if (tube == null) throw new ArgumentNullException("tube");
            if (tube.Length > MAX_TUBE_NAME_LENGTH) throw new ArgumentOutOfRangeException("tube", "Tube names are limited to " + MAX_TUBE_NAME_LENGTH + " ASCII bytes");
            if (!namePattern.IsMatch(tube)) throw new ArgumentOutOfRangeException("tube", "Tube name contains invalid characters");
            Name = tube;
        }

        public static implicit operator Tube(string tube)
        {
            return new Tube(tube);
        }
    }
}
