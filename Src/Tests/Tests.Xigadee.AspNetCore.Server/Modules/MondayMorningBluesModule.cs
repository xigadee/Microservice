using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Test.Xigadee;
using Xigadee;

namespace Tests.Xigadee
{
    public class MondayMorningBluesModule:ApiModuleBase
    {
        [RepositoryLoad]
        public IRepositoryAsync<Guid, MondayMorningBlues> RepositoryMondayMorningBlues { get; set; }

        public override Task Start(CancellationToken cancellationToken)
        {
            return base.Start(cancellationToken);
        }
    }
}
