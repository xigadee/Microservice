//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using System;

//namespace Xigadee
//{
//    /// <summary>
//    /// This is the standard api controller configuration.
//    /// </summary>
//    public class XigadeeAspNetPipelineControllerWithViews : XigadeeAspNetPipelineController
//    {
//        public XigadeeAspNetPipelineControllerWithViews(params Type[] types) : base(types)
//        {

//        }

//        public override void ConfigureControllerOptions(IServiceCollection services)
//        {
//            //Add the default API controller options.
//            var mvcBuilder = services.AddControllersWithViews();

//            //Add the child assemblies.
//            _types?.ForEach(t =>
//            {
//                mvcBuilder.AddApplicationPart(t.Assembly);
//            });


//            //mvcBuilder.AddMvcLocalization();

//            //Ensure the razor elements are compiled in to the application.
//            mvcBuilder.AddRazorRuntimeCompilation();
//        }

//        public override void ConfigurePipeline(IApplicationBuilder app)
//        {
//            app.UseDeveloperExceptionPage();

//            app.UseHttpsRedirection();

//            app.UseStaticFiles();
//        }

//    }
//}
