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
    public class SignMe { }

    [TestClass]
    public class EntityHintsCheck
    {
        [EntitySignatureHint(typeof(SignMe))]
        private class TestClass
        {
            [EntityIdHint]
            public Guid Id { get; set; } = Guid.NewGuid();

            [EntityVersionHint]
            public Guid VersionId { get; set; } = Guid.NewGuid();

            /// <summary>
            /// Gets or sets the unique name.
            /// </summary>
            [EntityReferenceHint("name")]
            public string Name { get; set; }

            [EntityPropertyHint("type")]
            public string Type { get; set; }

            [EntityPropertyHint("datecreated")]
            public DateTime DateCreated { get; set; } = DateTime.UtcNow;
            
            public DateTime? DateUpdated { get; set; }

            [EntityPropertyHint("dateyo")]
            public string TheDateToday() => DateCreated.ToString("o");
        }

        [TestMethod]
        public void Test1()
        {
            var t = new TestClass();
            t.Name = "fredo";
            t.Type = "NiceOne!";

            var resolver = EntityHintHelper.Resolve(t.GetType());

            Guid id = resolver.Id<Guid>(t);
            var version = resolver.VersionGet(t);

            var r = resolver.References(t).ToList();
            var p = resolver.Properties(t).ToList();
        }
    }
}
