using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Linq.Expressions;

namespace Tests.Xigadee
{
    [TestClass]
    public class TestArrayHelper
    {

        [TestMethod]
        public void Arrays()
        {
            //ArrayHelper.BinarySearchExt

            var result0 = TestEnum.Three.IsInFlag(TestEnum.Three);

            var result1 = TestEnum.Three.IsInFlag(TestEnum.One);
            var result2 = TestEnum.One.IsInFlag(TestEnum.Three);

            var resultn0 = TestEnum.Three.IsNotInFlag(TestEnum.Three);

            var resultn1 = TestEnum.Three.IsNotInFlag(TestEnum.One);
            var resultn2 = TestEnum.One.IsNotInFlag(TestEnum.Three);

        }
    }

    [Flags]
    public enum TestEnum
    {
        One = 1,
        Two = 2,
        Three = 3
    }
}
