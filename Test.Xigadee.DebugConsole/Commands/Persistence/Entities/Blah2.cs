using System;

namespace Test.Xigadee
{
    public class Blah2
    {
        public Blah2()
        {
            ContentId = Guid.NewGuid();
            VersionId = Guid.NewGuid();
        }

        public Guid ContentId { get; set; }

        public Guid VersionId { get; set; }

        public string Message { get; set; }
    }
}
