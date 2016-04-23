using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class TestMicroservice:Microservice
    {

        protected override TaskManager InitialiseTaskManager()
        {
            var tm = base.InitialiseTaskManager();

            tm.DiagnosticOnExecuteTaskBefore += Tm_DiagnosticOnExecuteTaskBefore;        
            return tm;
        }

        private void Tm_DiagnosticOnExecuteTaskBefore(object sender, TaskTracker e)
        {

        }
    }
}
