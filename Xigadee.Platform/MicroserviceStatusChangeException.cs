using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class MicroserviceStatusChangeException: Exception
    {
        public MicroserviceStatusChangeException(string message, Exception ex):base(message, ex) 
        
        {
        }
    }
}
