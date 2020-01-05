using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Tests.Xigadee
{

    [TestClass]
    public class PropertyBagTests
    {
        private class TestClass : IPropertyBag
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            /// <summary>
            /// Gets or sets the unique name.
            /// </summary>
            public string Name => this.PropertiesGetRaw("name");

            public int? Count => this.PropertiesGet<int?>("count");

            public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        }

        [TestMethod]
        public async Task Test1()
        {
            var item = new TestClass();

            item.PropertiesSet("count", 2);

            Assert.IsTrue(item.Count == 2);

            item.PropertiesSetRaw("count", null);
            
            Assert.IsTrue(!item.Count.HasValue);

        }
    }
}
