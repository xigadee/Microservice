using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class CommandRegisterEventArgs: EventArgs
    {
        public CommandRegisterEventArgs(Microservice service, ConfigConsole config)
        {
            Service = service;
            Config = config;
        }

        public Microservice Service { get; set; }

        public ConfigConsole Config { get; set; }
    }
}
