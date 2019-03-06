using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xigadee;

namespace Tests.Xigadee
{
    [TestClass]
    public class Base36
    {
        [TestMethod]
        public void TestGuid()
        {
            var id = Guid.NewGuid();

            long test = 487298;

            var check = test.ToString().LuhnCheckGenerate();

            var ln = BitConverter.GetBytes(test);
            var hmm = ln.ToAnyBase("0123456789".ToCharArray());
            var hmmB33 = ln.ToAnyBase(ConversionHelper.Alphabet36);
            var hmmB62 = ln.ToAnyBase(ConversionHelper.Alphabet62);


            var b64 = Convert.ToBase64String(id.ToByteArray());
            var b36 = id.ToBase62();
            var b16 = id.ToByteArray().ToAnyBase(ConversionHelper.Alphabet16);
            var gs = id.ToString("N");
        }
    }
}
