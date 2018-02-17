using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace Xigadee
{
    public static class ASPNetCorePipeline
    {
        //
        // Summary:
        //     Captures synchronous and asynchronous System.Exception instances from the pipeline
        //     and generates HTML error responses.
        //
        // Parameters:
        //   app:
        //     The Microsoft.AspNetCore.Builder.IApplicationBuilder.
        //
        // Returns:
        //     A reference to the app after the operation has completed.
        public static IApplicationBuilder UseXigadee(this IApplicationBuilder app)
        {
            return app;
        }

    }
}
