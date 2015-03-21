using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator Tube(string tube)
        {
            if (tube == null) throw new ArgumentNullException("tube");
            if (tube.Length > MAX_TUBE_NAME_LENGTH) throw new ArgumentOutOfRangeException("tube", "Tube names are limited to " + MAX_TUBE_NAME_LENGTH + " ASCII bytes");
            return new Tube { Name = tube };
        }
    }
}
