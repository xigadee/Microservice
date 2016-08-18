using System;
using System.Diagnostics;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {


        static void StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var serv = sender as IPopulatorConsole;

            sMenuMain.Value.AddInfoMessage($"{serv.Name}={e.StatusNew.ToString()}{e.Message}", true);
        }
    }
}
