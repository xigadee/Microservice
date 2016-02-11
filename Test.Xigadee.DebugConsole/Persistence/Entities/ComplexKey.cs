using System;
using Xigadee.Api;

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

    public class ComplexKeyMapper : KeyMapper<ComplexKey>
    {

        public ComplexKeyMapper():base(null,null)
        {
            mSerializer = (k) => string.Format("{0}|{1}", k.One, k.Two);
            mDeserializer = (s) =>
                {
                    var parts = s.Split('|');

                    var key = new ComplexKey();
                    key.One = int.Parse(parts[0]);
                    key.Two = new Guid(parts[1]);
                    return key;
                };
        }
    }

    public class Complex
    {
        public Complex()
        {
            One = DateTime.Now.Second;
            Two = Guid.NewGuid();
            Message = DateTime.Now.ToString();
        }


        public int One { get; set; }

        public Guid Two { get; set; }

        public string Message { get; set; }

        public ComplexKey ToKey()
        {
            return new ComplexKey() { One = One, Two = Two };
        }
    }
}
