using System;

namespace Xigadee
{
    public class MicroserviceStatusChangeException: Exception
    {
        public MicroserviceStatusChangeException(string message, Exception ex):base(message, ex)      
        {
        }
    }
}
