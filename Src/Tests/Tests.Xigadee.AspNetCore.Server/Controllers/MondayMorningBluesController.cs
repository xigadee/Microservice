using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xigadee;
using Test.Xigadee;
using Microsoft.Extensions.Logging;

namespace Tests.Xigadee
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class MondayMorningBluesController : EntityControllerRoot<Guid, MondayMorningBlues>
    {
        public MondayMorningBluesController(ILogger<MondayMorningBluesController> logger
            , IRepositoryAsync<Guid, MondayMorningBlues> repository, RepositoryKeyManager<Guid> keyManager = null) 
            : base(logger, repository, keyManager)
        {

        }

        [HttpPost()]
        public Task<IActionResult> Create([FromBody] MondayMorningBlues rq)
        {
            return base.CreateRoot(rq);
        }

        [Route("")]
        [Route("{id1}")]
        [HttpGet]
        public Task<IActionResult> Read(EntityRequestModel input)
        {
            return base.ReadRoot(input);
        }

        [HttpPut()]
        public Task<IActionResult> Update([FromBody] MondayMorningBlues rq)
        {
            return base.UpdateRoot(rq);
        }

        [Route("")]
        [Route("{id1}")]
        [HttpDelete]
        public Task<IActionResult> Delete(EntityRequestModel input)
        {
            return base.DeleteRoot(input);
        }

        [Route("")]
        [Route("{id1}")]
        [HttpHead]
        public Task<IActionResult> Version(EntityRequestModel input)
        {
            return base.VersionRoot(input);
        }

        [Route("searchentity")]
        [HttpGet]
        public Task<IActionResult> SearchEntity(SearchRequestModel input)
        {
            return base.SearchEntityRoot(input);
        }

        [Route("search")]
        [HttpGet]
        public Task<IActionResult> Search(SearchRequestModel input)
        {
            return base.SearchRoot(input);
        }
    }
}