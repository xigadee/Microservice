using System;
using System.Collections.Generic;

namespace Xigadee
{
    public abstract class ConsoleConfigurationBase
    {
        protected ConsoleConfigurationBase(string[] args)
        {
            Notifications = new List<ErrorInfo>();
            try
            {
                Switches = args.CommandArgsParse();
            }
            catch (Exception)
            {
                Switches = new Dictionary<string, string>();
            }
        }

        public List<ErrorInfo> Notifications { get; private set; }

        public Dictionary<string, string> Switches { get; private set; }
    }
}
