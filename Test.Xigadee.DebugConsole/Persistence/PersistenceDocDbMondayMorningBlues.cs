using System;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceDocDbMondayMorningBlues: PersistenceManagerHandlerDocumentDb<Guid, MondayMorningBlues>
    {
        public PersistenceDocDbMondayMorningBlues(DocumentDbConnection connection, string name, VersionPolicy<MondayMorningBlues> versionPolicy = null)
            : base(connection, name, (k) => k.Id
            , versionPolicy: versionPolicy
            , referenceMaker: References)
        {

        }

        static IEnumerable<Tuple<string, string>> References(MondayMorningBlues entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.Email))
                return new[] { new Tuple<string, string>("email", entity.Email) };

            return new Tuple<string, string>[] { };
        }
    }
}
