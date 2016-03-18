using System;

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

        public bool NotEnoughCoffee { get; set; }

        public bool NotEnoughSleep { get; set; }

        public Guid ContentId { get; set; }

        public Guid VersionId { get; set; }

        public string Message { get; set; }

        public string Email { get; set; }
    }
}
