using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    public class ComplexEntity
    {
        public ComplexEntity()
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
