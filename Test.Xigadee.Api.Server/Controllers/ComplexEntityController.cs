using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Xigadee;

namespace Test.Xigadee.Api.Server.Controllers
{
    [MediaTypeConverter(typeof(JsonTransportSerializer<ComplexEntity>))]
    public class ComplexEntityController: ApiPersistenceControllerAsyncBase<ComplexKey, ComplexEntity>
    {
        public ComplexEntityController(IRepositoryAsync<ComplexKey, ComplexEntity> respository) : base(respository)
        {
        }
    }
}