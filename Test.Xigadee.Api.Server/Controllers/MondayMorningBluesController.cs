using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Xigadee;

namespace Test.Xigadee.Api.Server.Controllers
{
    public class MondayMorningBluesController: ApiPersistenceControllerAsyncBase<Guid, MondayMorningBlues>
    {
        public MondayMorningBluesController(IRepositoryAsync<Guid, MondayMorningBlues> respository) : base(respository)
        {
        }
    }
}