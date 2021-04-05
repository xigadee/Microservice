using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                //.UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(k => k.AddServerHeader = false);
                    webBuilder.UseStartupXigadee<StartupContext>();
                })
            .Build()
            .Run();
        }

    }
}
