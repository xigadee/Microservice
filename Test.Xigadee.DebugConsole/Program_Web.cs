using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static void MicroserviceWebAPIStart()
        {
            //sServerContext.Client.Service.StatusChanged += ClientStatusChanged;

            //sServerContext.Client.Populate(ResolveClientSetting, true);
            //sServerContext.Client.Start();
        }
        static void MicroserviceWebAPIStop()
        {
            //sServerContext.Client.Service.StatusChanged += ClientStatusChanged;

            //sServerContext.Client.Populate(ResolveClientSetting, true);
            //sServerContext.Client.Start();
        }

        //static IDisposable mWebApp;
        //static ConsoleOption MicroserviceMenuWeb<I>(string configName)
        //    where I : IPopulator, new()
        //{
        //    var holder = new MicroserviceHolder<I>(sSwitches.ContainsKey("enviro") ? sSwitches["enviro"] : "Staging", configName);

        //    holder.Menu = new ConsoleMenu(holder.Title
        //    , new ConsoleOption("Start Service"
        //            , (m, o) =>
        //            {

        //                //string baseAddress = "http://localhost:29000/";
        //                //StartOptions opts = new StartOptions(baseAddress);
        //                //opts.Settings.Add("Paul", "FeckYou!");
        //                //opts.ServerFactory = "";
        //                ////opts.AppStartup = typeof(Ximura.Bff.Startup).AssemblyQualifiedName;
        //                //mWebApp = WebApp.Start<Ximura.Bff.Startup>(opts);
        //                ////holder.Start();
        //                ////mWebApp = WebApp.Start(opts);
        //                //m.Refresh();

        //                Task.Run(() =>
        //                {
        //                    string baseAddress = "http://localhost:29000/";
        //                    StartOptions opts = new StartOptions(baseAddress);
        //                    //opts.Settings.
        //                    mWebApp = WebApp.Start<Test.Xigadee.Api.Server.Startup>(opts);
        //                    //ValueRetail.Api.Bff.Startup

        //                    //holder.Microservice.StatusChanged += Microservice_StatusChanged;
        //                    //holder.Start(sSwitches.ContainsKey("enviro") ? sSwitches["enviro"] : "Staging");
        //                    m.Refresh();
        //                });
        //            }
        //            , enabled: (m, o) => !holder.IsActive
        //        )
        //        , new ConsoleOption("Stop Service"
        //            , (m, o) =>
        //            {
        //                holder.Stop();
        //                holder.Microservice.StatusChanged -= Microservice_StatusChanged;
        //                m.Refresh();
        //            }
        //            , enabled: (m, o) => holder.IsActive
        //        )
        //        );

        //    holder.Option = new ConsoleOption(holder.Title, holder.Menu);

        //    return holder.Option;
        //}

    }
}
