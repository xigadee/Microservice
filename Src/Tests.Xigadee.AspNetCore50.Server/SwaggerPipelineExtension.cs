using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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

        public string Name => "My API V1";

        public override void ConfigureServices(XigadeeAspNetPipelineExtensionScope scope, IServiceCollection services)
        {
            services.AddSwaggerGen();
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
