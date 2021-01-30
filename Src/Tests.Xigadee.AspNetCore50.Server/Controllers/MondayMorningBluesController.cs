using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xigadee;
using Test.Xigadee;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Tests.Xigadee.AspNetCore50
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize(Policy = "adminp")]
    [EntityControllerFeaturesSupport(EntityControllerFeatures.All)]
    public class MondayMorningBluesController : EntityController<Guid, MondayMorningBlues>
    {
        readonly MondayMorningBluesModule _mmb;

        public MondayMorningBluesController(ILogger<MondayMorningBluesController> logger
            , MondayMorningBluesModule mmb
            , RepositoryKeyManager<Guid> keyManager = null
            ) 
            : base(logger, mmb.RepositoryMondayMorningBlues, keyManager)
        {
            _mmb = mmb;
        }

        public override Task<IActionResult> Read(CombinedRequestModel input)
        {
            return base.Read(input);
        }
    }
}