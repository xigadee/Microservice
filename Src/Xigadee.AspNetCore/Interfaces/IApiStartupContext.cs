using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This is the root API-based microservice application interface.
    /// </summary>
    public interface IApiStartupContext: Microsoft.Extensions.Hosting.IHostedService, IApiStartupContextBase
    {
    }
}
