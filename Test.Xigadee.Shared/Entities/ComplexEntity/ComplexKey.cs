using System;
using Xigadee;

namespace Test.Xigadee
{
    [KeyMapper(typeof(ComplexKeyMapper))]
    public class ComplexKey:IEquatable<ComplexKey>
    {
        public int One { get; set; }
        public Guid Two { get; set; }

        public bool Equals(ComplexKey other)
        {
            if (other == null)
                return false;

            return other.One == this.One && other.Two == this.Two;
        }
    }
}
