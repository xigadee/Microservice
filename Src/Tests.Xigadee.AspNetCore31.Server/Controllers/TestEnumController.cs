using Microsoft.Extensions.Logging;
using Xigadee;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System;

namespace Tests.Xigadee
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TestEnumController : EnumControllerBase
    {
        public TestEnumController(ILogger<TestEnumController> logger) : base(logger)
        {

        }

        /// <summary>
        /// This is the list of supported account permissions.
        /// </summary>
        /// <returns>Returns the list of items.</returns>
        [HttpGet("someenum")]
        public IActionResult SomeEnumOrOther() => BuildEnumList<SomeEnumOrOther>(TimeSpan.FromDays(10));
    }

    [Description("A headline description.")]
    public enum SomeEnumOrOther
    {
        Paul,
        Is,
        [LocalizedDescription("GSD", "en-uk_GSD", "en-gb")]
        Doing,
        This,
        For,
        The,
        [Category("Making life complicated")]
        [Description("Money")]
        Love,
        [Description("In")]
        Of,
        Code
    }
}
