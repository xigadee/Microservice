﻿using System;
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
    public class MondayMorningBluesController : EntityController<Guid, MondayMorningBlues>
    {
        public MondayMorningBluesController(ILogger<MondayMorningBluesController> logger
            , IRepositoryAsync<Guid, MondayMorningBlues> repository, RepositoryKeyManager<Guid> keyManager = null) 
            : base(logger, repository, keyManager)
        {

        }


    }
}