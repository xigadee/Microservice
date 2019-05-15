using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Xigadee;

namespace Tests.Xigadee
{
    [TestClass]
    public class SqlJsonTests
    {

        [TestMethod]
        public void JsonSearchTest()
        {
            var sr = (SearchRequest)"$top=100&$id=default&$skip=3&$select=Type,Group,DateCreated&$orderby=Group desc, Type desc";

        }
    }
}
