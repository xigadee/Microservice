using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the standard api controller configuration.
    /// </summary>
    public class XigadeeAspNetPipelineControllerWithViews : XigadeeAspNetPipelineController
    {
        public XigadeeAspNetPipelineControllerWithViews(params Type[] types) : base(types)
        {

        }

        public override void ConfigureControllerOptions(IServiceCollection services)
        {
            //Add the default API controller options.
            var mvcBuilder = services.AddControllersWithViews();

            //Add the child assemblies.
            _types?.ForEach(t =>
            {
                mvcBuilder.AddApplicationPart(t.Assembly);
            });


            //mvcBuilder.AddMvcLocalization();

            //Ensure the razor elements are compiled in to the application.
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        public override void ConfigurePipeline(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
        }

        public override void ConfigureUseEndpoints(IApplicationBuilder app, IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }
    }
}
