using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class SearchFilterCheck
    {
        /// Eq Equal	/Suppliers?$filter=Address/City eq 'Redmond'
        /// Ne Not equal	/Suppliers?$filter=Address/City ne 'London'
        /// Gt Greater than	/Products?$filter=Price gt 20
        /// Ge Greater than or equal	/Products?$filter=Price ge 10
        /// Lt Less than	/Products?$filter=Price lt 20
        /// Le Less than or equal	/Products?$filter=Price le 100
        /// And Logical and	/Products?$filter=Price le 200 and Price gt 3.5
        /// Or Logical or	/Products?$filter=Price le 3.5 or Price gt 200
        /// Not Logical negation	/Products?$filter=not endswith(Description,'milk')
        [TestMethod]
        public void Filter1()
        {
            var sr = (SearchRequest)"$filter=Address eq 'Redmond' and Price gt 200";
        }
    }
}
