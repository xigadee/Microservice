using System;
using Xigadee;

namespace Test.Xigadee
{
    public class MondayMorningBlues
    {
        public MondayMorningBlues()
        {
            Id = Guid.NewGuid();
            ContentId = Guid.NewGuid();
            VersionId = Guid.NewGuid();
        }

        [EntityIdHint]
        public Guid Id { get; set; }

        [EntityPropertyHint("obiwan")]
        public int ObiWan { get; set; }

        [EntityPropertyHint("coffee")]
        public bool NotEnoughCoffee { get; set; }

        [EntityPropertyHint("sleep")]
        public bool NotEnoughSleep { get; set; }

        public Guid ContentId { get; set; }

        [EntityVersionHint]
        public Guid VersionId { get; set; }

        public string Message { get; set; }
        [EntityReferenceHint("email")]
        [EntityPropertyHint("email")]
        public string Email { get; set; }

        public Name UserName { get; set; }
    }

    public class Name
    {
        [EntityPropertyHint("namefirst")]
        public string NameFirst { get; set; }
        [EntityPropertyHint("namefamily")]
        public string NameFamily { get; set; }
    }
}
