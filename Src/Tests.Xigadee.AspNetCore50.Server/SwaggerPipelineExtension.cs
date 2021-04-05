using System;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Localization;
using Swashbuckle.AspNetCore.Annotations;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50.Server
{
    /// <summary>
    /// This class is used to insert the swagger definition.
    /// </summary>
    public class SwaggerPipelineExtension : XigadeeAspNetPipelineExtensionBase
    {
        public override bool Enabled => true;

        public string Path => "/swagger/v1/swagger.json";

        public string Name => "Xigadee.AspNetCore50 Test API";

        public override void ConfigureServices(XigadeeAspNetPipelineExtensionScope scope, IServiceCollection services)
        {
            //See: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/Basic/Startup.cs#L39
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = Name,
                        Version = "v1",
                        Description = "A sample API for testing Swashbuckle",
                        TermsOfService = new Uri("http://xigadee.org")
                    }
                );

                c.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, "Tests.Xigadee.AspNetCore50.Server.xml"));

                c.EnableAnnotations();
            });
        }

        public override void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Path, Name);
            });
        }

    }
}
