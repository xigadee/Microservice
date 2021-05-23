using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Test.Xigadee;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50
{
    /// <summary>
    /// This is the default test module.
    /// </summary>
    public class MondayMorningBluesModule : ApiModuleBase<StartupContext>
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MondayMorningBluesModule()
        {

        }

        /// <summary>
        /// This is the default repository.
        /// </summary>
        [RepositoryLoad]
        public IRepositoryAsync<Guid, MondayMorningBlues> RepositoryMondayMorningBlues { get; set; }


        /// <summary>
        /// This is the override for starting the module.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task Start(CancellationToken cancellationToken)
        {
            return base.Start(cancellationToken);
        }
    }

}
