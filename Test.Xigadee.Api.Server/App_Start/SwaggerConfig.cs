using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swashbuckle.Application;

namespace Test.Xigadee.Api.Server
{
    public class SwaggerConfig
    {

        public static void Register(PopulatorWebApi Service)
        {
            // /swagger/ui/index
            Service.ApiConfig.EnableSwagger(c =>
            {
                c.IncludeXmlComments("docs.XML");
                c.Schemes(new[] { "http", "https" });
                c.SingleApiVersion("1.0", "Xigadee Test API");
            })
            .EnableSwaggerUi();
        }

    }
}