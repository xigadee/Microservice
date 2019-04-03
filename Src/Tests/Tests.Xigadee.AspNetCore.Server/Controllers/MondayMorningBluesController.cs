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

namespace Tests.Xigadee
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize(Policy = "adminp")]
    [EntityControllerFeaturesSupport(EntityControllerFeatures.All)]
    public class MondayMorningBluesController : EntityController<Guid, MondayMorningBlues>
    {
        public MondayMorningBluesController(ILogger<MondayMorningBluesController> logger
            , IRepositoryAsync<Guid, MondayMorningBlues> repository, RepositoryKeyManager<Guid> keyManager = null) 
            : base(logger, repository, keyManager)
        {

        }

        public override Task<IActionResult> Read(CombinedRequestModel input)
        {
            return base.Read(input);
        }
    }
}