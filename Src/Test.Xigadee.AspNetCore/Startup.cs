using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xigadee;

namespace Test.Xigadee.AspNetCore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        DebugMemoryDataCollector mColl;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddXigadee()
                .AddSchedule((s) =>
                {
                    s.SetNextPollTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }
                //, new ScheduleTimerConfig(TimeSpan.FromSeconds(2))
                )
                ;


            //services.AddIdentity<MyUser,
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseXigadeeHttpBoundaryLogging();

            var serv = app.GetXigadeePipeline()
                .AddDebugMemoryDataCollector(out mColl);

            app.UseMvc();
        }
    }

    public class MyUser: IdentityUser
    {

    }
}
