using System;
using System.Collections.Generic;
using System.Text;
using Xigadee;

namespace Tests.Xigadee
{
    [EntitySignatureHint(typeof(TestClassSignaturePolicy))]
    public class TestClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VersionId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateUpdated { get; set; }
    }
}
