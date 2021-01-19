using System;
using Xigadee;

namespace Test.Xigadee
{
    public class Blah: IEquatable<Blah>
    {
        public Blah()
        {
            ContentId = Guid.NewGuid();
            VersionId = Guid.NewGuid();
        }

        public Guid ContentId { get; set; }

        public Guid VersionId { get; set; }

        public string Message { get; set; }

        public bool Equals(Blah other)
        {
            if (other == null)
                return false;

            return ContentId == other.ContentId
                && VersionId == other.VersionId
                && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            if (obj is Blah)
                return Equals((Blah)obj);

            return false;
        }
    }

    //public class BlahXmlTransportSerializer: XmlTransportSerializerBase<Blah>
    //{
    //    public override string XsdName
    //    {
    //        get { return "Test.Xigadee.Shared.Xsd.Blah.xsd"; }
    //    }
    //}
}
