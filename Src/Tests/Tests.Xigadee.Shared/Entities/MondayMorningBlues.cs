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

        public Guid Id { get; set; }

        public int ObiWan { get; set; }

        public bool NotEnoughCoffee { get; set; }

        public bool NotEnoughSleep { get; set; }

        public Guid ContentId { get; set; }

        public Guid VersionId { get; set; }

        public string Message { get; set; }

        public string Email { get; set; }

        public Name UserName { get; set; }
    }

    public class Name
    {
        public string NameFirst { get; set; }
        public string NameFamily { get; set; }
    }
}
