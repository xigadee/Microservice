using System;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static MondayMorningBlues CreateEntity(Guid? Id = null, Guid? versionId = null, string email = null)
        {
            Guid newId = Id ?? Guid.NewGuid();

            return new MondayMorningBlues
            {
                Id = newId,
                ContentId = newId,
                VersionId = versionId ?? Guid.NewGuid(),
                Message = DateTime.Now.ToString(),
                NotEnoughCoffee = true,
                NotEnoughSleep = true,
                Email = email
            };
        }

    }
}
